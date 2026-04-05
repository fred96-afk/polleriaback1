namespace Models.Clients;

public record ClientRequest(
    string Name,
    string? Phone,
    string? Address
);
