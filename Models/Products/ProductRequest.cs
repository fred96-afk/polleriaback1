using Microsoft.AspNetCore.Http;

namespace Models.Products;

public record ProductRequest(
    string Name,
    string? Description,
    decimal BasePrice,
    int? CategoryId,
    IFormFile? Image
);