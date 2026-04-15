using Microsoft.AspNetCore.Http;

namespace Models.Products;

public record ProductRequest(
    string Name,
    string? Description,
    decimal BasePrice,
    decimal? SalePrice,
    int? CategoryId,
    IFormFile? Image
);
