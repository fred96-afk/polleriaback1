using DbModel.Tables;

namespace IRepository;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Order>> GetAllWithIncludesAsync();
    Task<Order?> GetByTableNumberAsync(string tableNumber);
    Task<Order?> GetByIdWithIncludesAsync(int id);
}
