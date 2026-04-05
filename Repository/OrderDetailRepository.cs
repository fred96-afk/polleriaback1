using DbModel;
using DbModel.Tables;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class OrderDetailRepository(PolleriaDbContext context) : BaseRepository<OrderDetail>(context), IOrderDetailRepository
{
    public async Task<IEnumerable<OrderDetail>> GetByOrderIdWithIncludesAsync(int orderId)
    {
        return await _context.Set<OrderDetail>()
            .Include(d => d.Product)
            .Include(d => d.Side)
            .Where(d => d.OrderId == orderId)
            .ToListAsync();
    }
}
