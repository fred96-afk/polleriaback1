using Models.Products;
using Models.Common;

namespace IBusiness;

public interface IProductBusiness
{
    Task<IEnumerable<ProductResponse>> GetAllAsync();
    Task<PagedResponse<ProductResponse>> GetPagedAsync(PaginationParams pagination, string? term = null);
    Task<ProductResponse?> GetByIdAsync(int id);
    Task<IEnumerable<ProductResponse>> SearchAsync(string term);
    Task<ProductResponse> CreateAsync(ProductRequest request);
    Task<bool> UpdateAsync(int id, ProductRequest request);
    Task<bool> DeleteAsync(int id);
}
