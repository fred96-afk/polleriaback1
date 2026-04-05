using DbModel.Tables;

namespace IRepository;

public interface IOrderDetailRepository : IBaseRepository<OrderDetail>
{
    Task<IEnumerable<OrderDetail>> GetByOrderIdWithIncludesAsync(int orderId);
}
