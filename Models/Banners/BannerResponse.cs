namespace Models.Banners;

public record BannerResponse(
    int Id,
    string Title,
    string? Subtitle,
    string ImageUrl,
    string? LinkUrl,
    int Order,
    bool IsActive
);
