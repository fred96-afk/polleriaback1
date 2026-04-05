using DbModel.Tables;
using IBusiness;
using IRepository;
using Models.Sides;

namespace Business;

public class SideBusiness(ISideRepository repository) : ISideBusiness
{
    public async Task<IEnumerable<SideResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return entities.Select(e => new SideResponse(e.Id, e.Name, e.Description, e.Price, e.Type));
    }

    public async Task<SideResponse?> GetByIdAsync(int id)
    {
        var e = await repository.GetByIdAsync(id);
        return e == null ? null : new SideResponse(e.Id, e.Name, e.Description, e.Price, e.Type);
    }

    public async Task<SideResponse> CreateAsync(SideRequest request)
    {
        var entity = new Side
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Type = request.Type
        };
        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();
        return new SideResponse(entity.Id, entity.Name, entity.Description, entity.Price, entity.Type);
    }

    public async Task<bool> UpdateAsync(int id, SideRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.Type = request.Type;

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
