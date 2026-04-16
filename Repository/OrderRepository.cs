using DbModel;
using DbModel.Tables;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class OrderRepository(PolleriaDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var normalizedStartDate = startDate.Date;
        var normalizedEndDateExclusive = endDate.TimeOfDay == TimeSpan.Zero
            ? endDate.Date.AddDays(1)
            : endDate.AddTicks(1);

        return await _context.Orders
            .FromSqlInterpolated($@"
                EXEC dbo.sp_GetOrdersByDateRange
                    @StartDate = {normalizedStartDate},
                    @EndDateExclusive = {normalizedEndDateExclusive}")
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithIncludesAsync()
    {
        return await _dbSet
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Side)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdWithIncludesAsync(int id)
    {
        return await _dbSet
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Side)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
