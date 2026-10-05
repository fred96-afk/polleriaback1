namespace Models.Users;

public record UserRequest(
    string Name,
    string Email,
    string Password,
    int RoleId
);
