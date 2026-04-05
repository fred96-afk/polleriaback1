namespace Models.Users;

public record UserResponse(
    int Id,
    string Name,
    string Email,
    int RoleId
);
