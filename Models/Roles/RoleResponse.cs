namespace Models.Roles;

public record RoleResponse(
    int Id,
    string Name,
    List<PermissionResponse> Permissions
);
