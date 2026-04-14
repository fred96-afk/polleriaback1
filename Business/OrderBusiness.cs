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
    IMercadoPagoBusiness mercadoPagoBusiness,
    INubeFactBusiness nubeFactBusiness,
    IPusherService pusherService) : IOrderBusiness
{
    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        var orders = await orderRepository.GetAllAsync();
        var response = new List<OrderResponse>();

        foreach (var order in orders)
        {
            var details = await detailRepository.GetByOrderIdWithIncludesAsync(order.Id);
            response.Add(MapToResponse(order, details));
        }

        return response;
    }

    public async Task<OrderResponse?> GetByIdAsync(int id)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order == null) return null;

        var details = await detailRepository.GetByOrderIdWithIncludesAsync(order.Id);
        return MapToResponse(order, details);
    }

    public async Task<OrderResponse> CreateAsync(OrderRequest request)
    {
        var order = new Order
        {
            ClientId = request.ClientId,
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
        if (request.DeliveryUserId.HasValue)
        {
            try
            {
                await pusherService.TriggerAsync("delivery-orders", "new-order", orderResponse);
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
        var paymentUrl = await mercadoPagoBusiness.CreatePaymentPreferenceAsync(orderResponse);

        return orderResponse with { PaymentUrl = paymentUrl };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order == null) return false;

        orderRepository.Remove(order);
        return await orderRepository.SaveChangesAsync() > 0;
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
                d.Subtotal)).ToList()
        );
    }
}
