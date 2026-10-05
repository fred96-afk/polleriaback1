using DbModel.Tables;

namespace IRepository;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IEnumerable<Product>> SearchAsync(string term);
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedWithCategoryAsync(int pageNumber, int pageSize, string? term = null);
}
