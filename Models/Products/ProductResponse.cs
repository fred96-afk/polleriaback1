namespace Models.Products;

public record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal BasePrice
);
