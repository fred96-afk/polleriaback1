using IBusiness;
using IRepository;
using Models.Roles;

namespace Business;

public class RoleBusiness(IRoleRepository repository) : IRoleBusiness
{
    public async Task<IEnumerable<RoleResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return entities.Select(e => new RoleResponse(e.Id, e.Name));
    }

    public async Task<RoleResponse?> GetByIdAsync(int id)
    {
        var e = await repository.GetByIdAsync(id);
        return e == null ? null : new RoleResponse(e.Id, e.Name);
    }
}
