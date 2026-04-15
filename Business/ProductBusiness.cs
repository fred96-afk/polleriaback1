using DbModel.Tables;
using IBusiness;
using IRepository;
using Models.Products;
using Models.Common;
using Microsoft.EntityFrameworkCore;

namespace Business;

public class ProductBusiness(IProductRepository repository, ICloudinaryService cloudinaryService) : IProductBusiness
{
    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return entities.Select(e => MapToResponse(e));
    }

    public async Task<PagedResponse<ProductResponse>> GetPagedAsync(PaginationParams pagination, string? term = null)
    {
        var (items, totalCount) = await repository.GetPagedWithCategoryAsync(pagination.PageNumber, pagination.PageSize, term);

        var dtos = items.Select(e => MapToResponse(e));
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

        return new PagedResponse<ProductResponse>(dtos, totalCount, totalPages, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var e = await repository.GetByIdAsync(id);
        return e == null ? null : MapToResponse(e);
    }

    public async Task<IEnumerable<ProductResponse>> SearchAsync(string term)
    {
        var entities = await repository.SearchAsync(term);
        return entities.Select(e => MapToResponse(e));
    }

    public async Task<ProductResponse> CreateAsync(ProductRequest request)
    {
        string? imageUrl = null;
        if (request.Image != null)
        {
            imageUrl = await cloudinaryService.UploadImageAsync(request.Image, "products");
        }

        var entity = new Product
        {
            Name = request.Name,
            Description = request.Description,
            BasePrice = request.BasePrice,
            SalePrice = request.SalePrice,
            CategoryId = request.CategoryId,
            ImageUrl = imageUrl
        };
        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, ProductRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        if (request.Image != null)
        {
            entity.ImageUrl = await cloudinaryService.UploadImageAsync(request.Image, "products");
        }

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.BasePrice = request.BasePrice;
        entity.SalePrice = request.SalePrice;
        entity.CategoryId = request.CategoryId;

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

    private static ProductResponse MapToResponse(Product e)
    {
        return new ProductResponse(
            e.Id, 
            e.Name, 
            e.Description, 
            e.BasePrice, 
            e.SalePrice, 
            e.IsOnSale, 
            e.CategoryId, 
            e.ImageUrl
        );
    }
}
