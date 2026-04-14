using Microsoft.AspNetCore.Http;

namespace Models.Banners;

public record BannerRequest(
    string Title,
    string? Subtitle,
    IFormFile? Image,
    string? LinkUrl,
    int Order,
    bool IsActive = true
);
