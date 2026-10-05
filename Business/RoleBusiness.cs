using DbModel.Tables;
using IBusiness;
using IRepository;
using Models.Roles;

namespace Business;

public class RoleBusiness(IRoleRepository repository, IPermissionRepository permissionRepository) : IRoleBusiness
{
    public async Task<IEnumerable<RoleResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllWithPermissionsAsync();
        return entities.Select(MapToResponse);
    }

    public async Task<RoleResponse?> GetByIdAsync(int id)
    {
        var e = await repository.GetByIdWithPermissionsAsync(id);
        return e == null ? null : MapToResponse(e);
    }

    public async Task<RoleResponse> CreateAsync(CreateRoleRequest request)
    {
        var role = new Role { Name = request.Name };
        await repository.AddAsync(role);
        await repository.SaveChangesAsync();

        if (request.PermissionIds.Any())
        {
            foreach (var pId in request.PermissionIds)
            {
                role.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = pId });
            }
            await repository.SaveChangesAsync();
        }

        var result = await repository.GetByIdWithPermissionsAsync(role.Id);
        return MapToResponse(result!);
    }

    public async Task<RoleResponse?> UpdateAsync(int id, UpdateRoleRequest request)
    {
        var role = await repository.GetByIdWithPermissionsAsync(id);
        if (role == null) return null;

        role.Name = request.Name;

        // Update permissions
        role.RolePermissions.Clear();
        foreach (var pId in request.PermissionIds)
        {
            role.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = pId });
        }

        repository.Update(role);
        await repository.SaveChangesAsync();

        return MapToResponse(role);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var role = await repository.GetByIdAsync(id);
        if (role == null) return false;

        repository.Remove(role);
        return await repository.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<PermissionResponse>> GetAllPermissionsAsync()
    {
        var permissions = await permissionRepository.GetAllAsync();
        return permissions.Select(p => new PermissionResponse(p.Id, p.Name, p.Code));
    }

    private static RoleResponse MapToResponse(Role role)
    {
        return new RoleResponse(
            role.Id,
            role.Name,
            role.RolePermissions.Select(rp => new PermissionResponse(
                rp.Permission.Id,
                rp.Permission.Name,
                rp.Permission.Code)).ToList()
        );
    }
}
