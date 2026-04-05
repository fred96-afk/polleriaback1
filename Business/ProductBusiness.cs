using DbModel.Tables;
using IBusiness;
using IRepository;
using Models.Products;

namespace Business;

public class ProductBusiness(IProductRepository repository) : IProductBusiness
{
    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return entities.Select(e => new ProductResponse(e.Id, e.Name, e.Description, e.BasePrice));
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var e = await repository.GetByIdAsync(id);
        return e == null ? null : new ProductResponse(e.Id, e.Name, e.Description, e.BasePrice);
    }

    public async Task<ProductResponse> CreateAsync(ProductRequest request)
    {
        var entity = new Product
        {
            Name = request.Name,
            Description = request.Description,
            BasePrice = request.BasePrice
        };
        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();
        return new ProductResponse(entity.Id, entity.Name, entity.Description, entity.BasePrice);
    }

    public async Task<bool> UpdateAsync(int id, ProductRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.BasePrice = request.BasePrice;

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
