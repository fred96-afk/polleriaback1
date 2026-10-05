namespace Models.Roles;

public record CreateRoleRequest(
    string Name,
    List<int> PermissionIds
);
