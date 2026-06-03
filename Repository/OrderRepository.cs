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
        var normalizedEndDate = endDate.Date.AddDays(1).AddTicks(-1);

        return await _dbSet
            .Include(o => o.Client)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .Where(o => o.OrderDate >= normalizedStartDate && o.OrderDate <= normalizedEndDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithIncludesAsync()
    {
        return await _dbSet
            .Include(o => o.Client)
            .Include(o => o.User)
            .Include(o => o.DeliveryUser)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Side)
            .OrderByDescending(o => o.OrderDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Order?> GetByTableNumberAsync(string tableNumber)
    {
        return await _dbSet
            .Include(o => o.Client)
            .Include(o => o.User)
            .Include(o => o.DeliveryUser)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Side)
            .Where(o => o.TableNumber == tableNumber && o.PaymentStatus == PaymentStatus.Pending && o.Status != OrderStatus.Cancelled)
            .OrderByDescending(o => o.OrderDate)
            .FirstOrDefaultAsync();
    }

    public async Task<Order?> GetByIdWithIncludesAsync(int id)
    {
        return await _dbSet
            .Include(o => o.Client)
            .Include(o => o.User)
            .Include(o => o.DeliveryUser)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Side)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
