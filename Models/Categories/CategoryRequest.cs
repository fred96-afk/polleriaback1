using Microsoft.AspNetCore.Http;

namespace Models.Categories;

public record CategoryRequest(
    string Name,
    string? Description,
    IFormFile? Image
);