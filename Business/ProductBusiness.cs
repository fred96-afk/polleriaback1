using DbModel.Tables;
using IBusiness;
using IRepository;
using Models.Products;

namespace Business;

public class ProductBusiness(IProductRepository repository, ICloudinaryService cloudinaryService) : IProductBusiness
{
    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return entities.Select(e => new ProductResponse(e.Id, e.Name, e.Description, e.BasePrice, e.CategoryId, e.ImageUrl));
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var e = await repository.GetByIdAsync(id);
        return e == null ? null : new ProductResponse(e.Id, e.Name, e.Description, e.BasePrice, e.CategoryId, e.ImageUrl);
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
            CategoryId = request.CategoryId,
            ImageUrl = imageUrl
        };
        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();
        return new ProductResponse(entity.Id, entity.Name, entity.Description, entity.BasePrice, entity.CategoryId, entity.ImageUrl);
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
}