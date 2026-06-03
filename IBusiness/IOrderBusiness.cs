using Models.Orders;

namespace IBusiness;

public interface IOrderBusiness
{
    Task<IEnumerable<OrderResponse>> GetAllAsync();
    Task<IEnumerable<OrderResponse>> GetDeliveryOrdersAsync();
    Task<OrderResponse?> GetByTableNumberAsync(string tableNumber);
    Task<OrderResponse?> GetByIdAsync(int id);
    Task<OrderResponse?> GetTrackingAsync(int id);
    Task<OrderResponse> CreateAsync(OrderRequest request);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateStatusAsync(int id, string status);
    Task<bool> UpdatePaymentStatusAsync(int id, string status);
    Task<bool> AcceptDeliveryOrderAsync(int id, int deliveryUserId);
}
