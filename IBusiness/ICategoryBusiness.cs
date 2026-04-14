using Models.Categories;
using Models.Common;

namespace IBusiness;

public interface ICategoryBusiness
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync();
    Task<PagedResponse<CategoryResponse>> GetPagedAsync(PaginationParams pagination);
    Task<CategoryResponse?> GetByIdAsync(int id);
    Task<CategoryResponse> CreateAsync(CategoryRequest request);
    Task<bool> UpdateAsync(int id, CategoryRequest request);
    Task<bool> DeleteAsync(int id);
}