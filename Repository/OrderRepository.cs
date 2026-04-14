using DbModel;
using DbModel.Tables;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class OrderRepository(PolleriaDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}
