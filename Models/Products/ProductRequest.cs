namespace Models.Products;

public record ProductRequest(
    string Name,
    string? Description,
    decimal BasePrice
);
