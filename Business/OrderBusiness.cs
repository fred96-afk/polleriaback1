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

    public async Task<IEnumerable<OrderResponse>> GetDeliveryOrdersAsync()
    {
        var orders = await orderRepository.GetAllWithIncludesAsync();
        // Filtrar solo pedidos de tipo Delivery
        return orders
            .Where(o => o.Type == OrderType.Delivery)
            .Select(o => MapToResponse(o, o.OrderDetails));
    }

    public async Task<OrderResponse?> GetByIdAsync(int id)
    {
        var order = await orderRepository.GetByIdWithIncludesAsync(id);
        if (order == null) return null;

        return MapToResponse(order, order.OrderDetails);
    }

    public async Task<OrderResponse?> GetTrackingAsync(int id)
    {
        // El rastreo usa la misma lógica que GetById pero puede ser usado públicamente
        var order = await orderRepository.GetByIdWithIncludesAsync(id);
        if (order == null) return null;

        return MapToResponse(order, order.OrderDetails);
    }

    public async Task<OrderResponse> CreateAsync(OrderRequest request)
    {
        Client? client = null;
        
        // 1. Buscar o Crear Cliente
        if (!string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            var existingClients = await clientRepository.FindAsync(c => c.DocumentNumber == request.DocumentNumber);
            client = existingClients.FirstOrDefault();
        }

        if (client == null && !string.IsNullOrWhiteSpace(request.CustomerName))
        {
            client = new Client
            {
                Name = request.CustomerName,
                DocumentNumber = request.DocumentNumber,
                DocumentType = request.DocumentType ?? "DNI",
                Email = request.CustomerEmail,
                Address = request.CustomerAddress ?? "-",
                Phone = request.CustomerPhone
            };
            await clientRepository.AddAsync(client);
            await clientRepository.SaveChangesAsync();
        }
        else if (client != null)
        {
            // Actualizar datos si han cambiado
            bool updated = false;
            if (!string.IsNullOrWhiteSpace(request.CustomerName) && client.Name != request.CustomerName) { client.Name = request.CustomerName; updated = true; }
            if (!string.IsNullOrWhiteSpace(request.CustomerEmail) && client.Email != request.CustomerEmail) { client.Email = request.CustomerEmail; updated = true; }
            if (!string.IsNullOrWhiteSpace(request.CustomerAddress) && client.Address != request.CustomerAddress) { client.Address = request.CustomerAddress; updated = true; }
            if (!string.IsNullOrWhiteSpace(request.CustomerPhone) && client.Phone != request.CustomerPhone) { client.Phone = request.CustomerPhone; updated = true; }
            
            if (updated)
            {
                clientRepository.Update(client);
                await clientRepository.SaveChangesAsync();
            }
        }

        // Determinar el tipo de orden
        var orderType = OrderType.Delivery;
        if (request.IsPos) orderType = OrderType.POS;
        else if (request.IsPickup) orderType = OrderType.Pickup;

        // 2. Crear la Orden
        var order = new Order
        {
            ClientId = client?.Id,
            Client = client, // Seteamos la propiedad de navegación para asegurar que se incluya en la respuesta
            UserId = request.UserId,
            DeliveryUserId = request.DeliveryUserId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = 0,
            Type = orderType,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending
        };

        await orderRepository.AddAsync(order);
        await orderRepository.SaveChangesAsync();

        // 3. Procesar Detalles
        decimal totalAmount = 0;
        foreach (var item in request.Details)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product == null) continue;

            decimal unitPrice = product.BasePrice;
            if (item.SideId.HasValue)
            {
                var side = await sideRepository.GetByIdAsync(item.SideId.Value);
                if (side != null) unitPrice += side.Price;
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

        // 4. Obtener orden completa para respuesta
        var orderWithIncludes = await orderRepository.GetByIdWithIncludesAsync(order.Id);
        if (orderWithIncludes == null) throw new InvalidOperationException("Error al recuperar la orden creada.");

        var orderResponse = MapToResponse(orderWithIncludes, orderWithIncludes.OrderDetails);

        // 5. Notificaciones (Pusher)
        if (request.DeliveryUserId.HasValue || order.Status == OrderStatus.Pending)
        {
            try { await pusherService.TriggerAsync("orders", "new-order", orderResponse); }
            catch (Exception ex) { Console.WriteLine($"Error Pusher: {ex.Message}"); }
        }

        // 6. Facturación (NubeFact) si es POS
        if (request.IsPos)
        {
            try
            {
                var result = await nubeFactBusiness.GenerateInvoiceAsync(order.Id);
                if (!result.Success) throw new InvalidOperationException(result.Error ?? "Error NubeFact.");

                if (!string.IsNullOrWhiteSpace(orderResponse.CustomerEmail))
                {
                    try { await emailService.SendOrderInvoiceEmailAsync(orderResponse.CustomerEmail, orderResponse.CustomerName ?? "Cliente", order.Id.ToString(), order.TotalAmount, result.PdfUrl!); }
                    catch (Exception ex) { Console.WriteLine($"Error Email: {ex.Message}"); }
                }

                return orderResponse with { PdfUrl = result.PdfUrl };
            }
            catch (Exception ex) { Console.WriteLine($"Error NubeFact: {ex.Message}"); return orderResponse; }
        }

        // 7. Mercado Pago para Ventas Online
        // Usamos el email del cliente que ya viene en la respuesta procesada
        var payerEmail = !string.IsNullOrWhiteSpace(orderResponse.CustomerEmail) 
            ? orderResponse.CustomerEmail 
            : "cliente_polleria@test.com"; 

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
        var order = await orderRepository.GetByIdWithIncludesAsync(id);
        if (order == null) return false;

        // Normalización para compatibilidad con el front (OnWay -> OnTheWay)
        if (status.Equals("OnWay", StringComparison.OrdinalIgnoreCase))
        {
            status = "OnTheWay";
        }

        if (Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
        {
            order.Status = orderStatus;
            orderRepository.Update(order);
            var result = await orderRepository.SaveChangesAsync() > 0;

            if (result)
            {
                var response = MapToResponse(order, order.OrderDetails);
                await pusherService.TriggerAsync("orders", "status-updated", response);
            }

            return result;
        }

        return false;
    }

    public async Task<bool> UpdatePaymentStatusAsync(int id, string status)
    {
        var order = await orderRepository.GetByIdWithIncludesAsync(id);
        if (order == null) return false;

        if (Enum.TryParse<PaymentStatus>(status, true, out var paymentStatus))
        {
            order.PaymentStatus = paymentStatus;
            orderRepository.Update(order);
            var result = await orderRepository.SaveChangesAsync() > 0;

            if (result)
            {
                var response = MapToResponse(order, order.OrderDetails);
                await pusherService.TriggerAsync("orders", "payment-updated", response);
            }

            return result;
        }

        return false;
    }

    public async Task<bool> AcceptDeliveryOrderAsync(int id, int deliveryUserId)
    {
        var order = await orderRepository.GetByIdWithIncludesAsync(id);
        if (order == null) return false;

        order.DeliveryUserId = deliveryUserId;
        order.Status = OrderStatus.Accepted;
        orderRepository.Update(order);
        var result = await orderRepository.SaveChangesAsync() > 0;

        if (result)
        {
            var response = MapToResponse(order, order.OrderDetails);
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
            order.Client?.Name,
            order.Client?.Address,
            order.Client?.Phone,
            order.Client?.Email,
            order.UserId,
            order.User?.Name,
            order.DeliveryUserId,
            order.DeliveryUser?.Name,
            order.TotalAmount,
            order.Type.ToString(),
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
