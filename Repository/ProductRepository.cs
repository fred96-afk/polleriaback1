using DbModel;
using DbModel.Tables;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class ProductRepository(PolleriaDbContext context) : BaseRepository<Product>(context), IProductRepository
{
    public async Task<IEnumerable<Product>> SearchAsync(string term)
    {
        var query = _dbSet.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(term))
        {
            term = term.ToLower();
            query = query.Where(p => 
                p.Name.ToLower().Contains(term) || 
                (p.Category != null && p.Category.Name.ToLower().Contains(term)));
        }

        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedWithCategoryAsync(int pageNumber, int pageSize, string? term = null)
    {
        var query = _dbSet.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(term))
        {
            term = term.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                (p.Category != null && p.Category.Name.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
