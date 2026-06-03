using Models.Roles;

namespace IBusiness;

public interface IRoleBusiness
{
    Task<IEnumerable<RoleResponse>> GetAllAsync();
    Task<RoleResponse?> GetByIdAsync(int id);
    Task<RoleResponse> CreateAsync(CreateRoleRequest request);
    Task<RoleResponse?> UpdateAsync(int id, UpdateRoleRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<PermissionResponse>> GetAllPermissionsAsync();
}
