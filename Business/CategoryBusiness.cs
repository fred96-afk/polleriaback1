using DbModel.Tables;
using IBusiness;
using IRepository;
using Models.Categories;
using Models.Common;

namespace Business;

public class CategoryBusiness(ICategoryRepository repository, ICloudinaryService cloudinaryService) : ICategoryBusiness
{
    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return entities.Select(e => new CategoryResponse(e.Id, e.Name, e.Description, e.ImageUrl));
    }

    public async Task<PagedResponse<CategoryResponse>> GetPagedAsync(PaginationParams pagination)
    {
        var (items, totalCount) = await repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
        var dtos = items.Select(e => new CategoryResponse(e.Id, e.Name, e.Description, e.ImageUrl));
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

        return new PagedResponse<CategoryResponse>(dtos, totalCount, totalPages, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var e = await repository.GetByIdAsync(id);
        return e == null ? null : new CategoryResponse(e.Id, e.Name, e.Description, e.ImageUrl);
    }

    public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
    {
        string? imageUrl = null;
        if (request.Image != null)
        {
            imageUrl = await cloudinaryService.UploadImageAsync(request.Image, "categories");
        }

        var entity = new Category
        {
            Name = request.Name,
            Description = request.Description,
            ImageUrl = imageUrl
        };
        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();
        return new CategoryResponse(entity.Id, entity.Name, entity.Description, entity.ImageUrl);
    }

    public async Task<bool> UpdateAsync(int id, CategoryRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        if (request.Image != null)
        {
            entity.ImageUrl = await cloudinaryService.UploadImageAsync(request.Image, "categories");
        }

        entity.Name = request.Name;
        entity.Description = request.Description;

        repository.Update(entity);
        return await repository.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        repository.Remove(entity);
        return await repository.SaveChangesAsync() > 0;
    }
}