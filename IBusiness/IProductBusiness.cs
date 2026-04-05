using Models.Products;

namespace IBusiness;

public interface IProductBusiness
{
    Task<IEnumerable<ProductResponse>> GetAllAsync();
    Task<ProductResponse?> GetByIdAsync(int id);
    Task<ProductResponse> CreateAsync(ProductRequest request);
    Task<bool> UpdateAsync(int id, ProductRequest request);
    Task<bool> DeleteAsync(int id);
}
