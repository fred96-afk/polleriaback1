namespace Models.Clients;

public record ClientResponse(
    int Id,
    string Name,
    string? Phone,
    string? Address
);
