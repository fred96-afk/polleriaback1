using DbModel.Tables;
using IBusiness;
using IRepository;
using Models.Orders;

namespace Business;

public class OrderBusiness(
    IOrderRepository orderRepository,
    IOrderDetailRepository detailRepository,
    IProductRepository productRepository,
    ISideRepository sideRepository,
    IUserRepository userRepository,
    IClientRepository clientRepository,
    IMercadoPagoBusiness mercadoPagoBusiness,
    INubeFactBusiness nubeFactBusiness,
    IEmailService emailService,
    IPusherService pusherService) : IOrderBusiness
{
    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        var orders = await orderRepository.GetAllWithIncludesAsync();
        return orders.Select(o => MapToResponse(o, o.OrderDetails));
    }

    public async Task<OrderResponse?> GetByIdAsync(int id)
    {
        var order = await orderRepository.GetByIdWithIncludesAsync(id);
        if (order == null) return null;

        return MapToResponse(order, order.OrderDetails);
    }

    public async Task<OrderResponse> CreateAsync(OrderRequest request)
    {
        int? clientId = request.ClientId;
        string? customerEmail = request.CustomerEmail;

        // Lógica para POS: Nombre y DNI obligatorios
        if (request.IsPos)
        {
            if (string.IsNullOrWhiteSpace(request.DocumentNumber) || string.IsNullOrWhiteSpace(request.CustomerName))
            {
                throw new InvalidOperationException("El nombre del cliente y el número de documento son obligatorios para ventas POS.");
            }

            // Buscar si el cliente ya existe por documento
            var existingClients = await clientRepository.FindAsync(c => c.DocumentNumber == request.DocumentNumber);
            var client = existingClients.FirstOrDefault();

            if (client == null)
            {
                // Crear cliente nuevo si no existe
                client = new Client
                {
                    Name = request.CustomerName,
                    DocumentNumber = request.DocumentNumber,
                    DocumentType = request.DocumentType ?? "DNI",
                    Email = request.CustomerEmail,
                    Address = "-"
                };
                await clientRepository.AddAsync(client);
                await clientRepository.SaveChangesAsync();
            }
            else
            {
                // Actualizar email si se proporciona uno nuevo
                if (!string.IsNullOrWhiteSpace(request.CustomerEmail) && client.Email != request.CustomerEmail)
                {
                    client.Email = request.CustomerEmail;
                    clientRepository.Update(client);
                    await clientRepository.SaveChangesAsync();
                }
                
                if (string.IsNullOrWhiteSpace(customerEmail))
                {
                    customerEmail = client.Email;
                }
            }
            
            clientId = client.Id;
        }

        var order = new Order
        {
            ClientId = clientId,
            UserId = request.UserId,
            DeliveryUserId = request.DeliveryUserId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = 0
        };

        await orderRepository.AddAsync(order);
        await orderRepository.SaveChangesAsync();

        decimal totalAmount = 0;

        foreach (var item in request.Details)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product == null) continue;

            decimal unitPrice = product.BasePrice;
            if (item.SideId.HasValue)
            {
                var side = await sideRepository.GetByIdAsync(item.SideId.Value);
                if (side != null)
                {
                    unitPrice += side.Price;
                }
            }

            var subtotal = unitPrice * item.Quantity;
            totalAmount += subtotal;

            var detail = new OrderDetail
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                SideId = item.SideId,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                Subtotal = subtotal
            };

            await detailRepository.AddAsync(detail);
        }

        order.TotalAmount = totalAmount;
        orderRepository.Update(order);
        await orderRepository.SaveChangesAsync();

        // Obtener detalles con inclusiones para la respuesta y para MP
        var detailsWithIncludes = await detailRepository.GetByOrderIdWithIncludesAsync(order.Id);
        var orderResponse = MapToResponse(order, detailsWithIncludes);

        // Notificar por Pusher si es un pedido de tipo delivery
        if (request.DeliveryUserId.HasValue || order.Status == OrderStatus.Pending)
        {
            try
            {
                await pusherService.TriggerAsync("orders", "new-order", orderResponse);
            }
            catch (Exception ex)
            {
                // Log and continue, notification shouldn't break the order creation
                Console.WriteLine($"Error al enviar notificación Pusher: {ex.Message}");
            }
        }

        // Si es una venta de Punto de Venta (POS), generamos comprobante nubefact
        if (request.IsPos)
        {
            try
            {
                var result = await nubeFactBusiness.GenerateInvoiceAsync(order.Id);
                if (!result.Success)
                {
                    throw new InvalidOperationException(result.Error ?? "No se pudo generar el comprobante.");
                }

                // Enviar correo si tenemos email del cliente
                if (!string.IsNullOrWhiteSpace(customerEmail))
                {
                    try
                    {
                        await emailService.SendOrderInvoiceEmailAsync(
                            customerEmail,
                            request.CustomerName ?? "Cliente",
                            order.Id.ToString(),
                            order.TotalAmount,
                            result.PdfUrl!
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al enviar correo de comprobante: {ex.Message}");
                    }
                }

                return orderResponse with { PdfUrl = result.PdfUrl };
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error preventivo NubeFact: {ex.Message}");
                return orderResponse;
            }
        }

        // Generar enlace de pago para ventas online
        var user = await userRepository.GetByIdAsync(order.UserId);
        var payerEmail = user?.Email ?? "test_user_polleria@test.com"; // Email por defecto si no hay usuario

        var paymentUrl = await mercadoPagoBusiness.CreatePaymentPreferenceAsync(orderResponse, payerEmail);

        return orderResponse with { PaymentUrl = paymentUrl };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order == null) return false;

        orderRepository.Remove(order);
        return await orderRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order == null) return false;

        if (Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
        {
            order.Status = orderStatus;
            orderRepository.Update(order);
            var result = await orderRepository.SaveChangesAsync() > 0;

            if (result)
            {
                var details = await detailRepository.GetByOrderIdWithIncludesAsync(order.Id);
                var response = MapToResponse(order, details);
                await pusherService.TriggerAsync("orders", "status-updated", response);
            }

            return result;
        }

        return false;
    }

    public async Task<bool> UpdatePaymentStatusAsync(int id, string status)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order == null) return false;

        if (Enum.TryParse<PaymentStatus>(status, true, out var paymentStatus))
        {
            order.PaymentStatus = paymentStatus;
            orderRepository.Update(order);
            var result = await orderRepository.SaveChangesAsync() > 0;

            if (result)
            {
                var details = await detailRepository.GetByOrderIdWithIncludesAsync(order.Id);
                var response = MapToResponse(order, details);
                await pusherService.TriggerAsync("orders", "payment-updated", response);
            }

            return result;
        }

        return false;
    }

    public async Task<bool> AcceptDeliveryOrderAsync(int id, int deliveryUserId)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order == null) return false;

        order.DeliveryUserId = deliveryUserId;
        order.Status = OrderStatus.Accepted;
        orderRepository.Update(order);
        var result = await orderRepository.SaveChangesAsync() > 0;

        if (result)
        {
            var details = await detailRepository.GetByOrderIdWithIncludesAsync(order.Id);
            var response = MapToResponse(order, details);
            await pusherService.TriggerAsync("orders", "order-accepted", response);
        }

        return result;
    }

    private static OrderResponse MapToResponse(Order order, IEnumerable<OrderDetail> details)
    {
        return new OrderResponse(
            order.Id,
            order.OrderDate,
            order.ClientId,
            order.UserId,
            order.DeliveryUserId,
            order.TotalAmount,
            details.Select(d => new OrderDetailResponse(
                d.Id,
                d.ProductId,
                d.Product?.Name,
                d.SideId,
                d.Side?.Name,
                d.Quantity,
                d.UnitPrice,
                d.Subtotal)).ToList(),
            order.Status.ToString(),
            order.PaymentStatus.ToString()
        );
    }
}
