using Models.Roles;

namespace IBusiness;

public interface IRoleBusiness
{
    Task<IEnumerable<RoleResponse>> GetAllAsync();
    Task<RoleResponse?> GetByIdAsync(int id);
}
