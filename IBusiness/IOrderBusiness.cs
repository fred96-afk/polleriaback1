using Models.Orders;

namespace IBusiness;

public interface IOrderBusiness
{
    Task<IEnumerable<OrderResponse>> GetAllAsync();
    Task<OrderResponse?> GetByIdAsync(int id);
    Task<OrderResponse> CreateAsync(OrderRequest request);
    Task<bool> DeleteAsync(int id);
}
