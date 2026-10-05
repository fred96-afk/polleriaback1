namespace Models.Roles;

public record UpdateRoleRequest(
    string Name,
    List<int> PermissionIds
);
