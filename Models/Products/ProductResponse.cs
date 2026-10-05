namespace Models.Products;

public record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal BasePrice,
    decimal? SalePrice,
    bool IsOnSale,
    int? CategoryId,
    string? ImageUrl
);
