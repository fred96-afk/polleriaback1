namespace Models.Categories;

public record CategoryResponse(
    int Id,
    string Name,
    string? Description,
    string? ImageUrl
);