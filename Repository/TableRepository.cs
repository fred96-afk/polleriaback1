using DbModel;
using DbModel.Tables;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class TableRepository(PolleriaDbContext context) : BaseRepository<RestaurantTable>(context), ITableRepository
{
    public async Task<RestaurantTable?> GetByNumberAsync(string number)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Number.ToLower() == number.ToLower());
    }
}
