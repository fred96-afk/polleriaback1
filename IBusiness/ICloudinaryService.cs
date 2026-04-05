using Microsoft.AspNetCore.Http;

namespace IBusiness;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string folder = "polleria");
    Task<bool> DeleteImageAsync(string publicId);
}
