using IBusiness;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class ReportStorageService : IReportStorageService
{
    private readonly string _storageDirectory;
    private readonly ILogger<ReportStorageService> _logger;

    public ReportStorageService(ILogger<ReportStorageService> logger)
    {
        _logger = logger;
        _storageDirectory = Path.Combine(AppContext.BaseDirectory, "reports_storage");

        if (!Directory.Exists(_storageDirectory))
        {
            Directory.CreateDirectory(_storageDirectory);
            _logger.LogInformation("Directorio de almacenamiento de reportes creado en: {Path}", _storageDirectory);
        }
    }

    public async Task<string> SaveReportAsync(string fileName, byte[] data)
    {
        // Limpiar caracteres no válidos de nombre de archivo
        var safeFileName = Path.GetFileName(fileName);
        var fullPath = Path.Combine(_storageDirectory, safeFileName);

        await File.WriteAllBytesAsync(fullPath, data);
        _logger.LogInformation("Reporte guardado exitosamente: {FileName} ({Bytes} bytes)", safeFileName, data.Length);

        return safeFileName;
    }

    public async Task<byte[]?> GetReportAsync(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        var fullPath = Path.Combine(_storageDirectory, safeFileName);

        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("Reporte solicitado no encontrado en disco: {FileName}", safeFileName);
            return null;
        }

        return await File.ReadAllBytesAsync(fullPath);
    }

    public async Task<(byte[]? Data, string ContentType)> GetReportWithContentTypeAsync(string fileName)
    {
        var data = await GetReportAsync(fileName);
        var contentType = GetContentType(fileName);
        return (data, contentType);
    }

    public bool ReportExists(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        var fullPath = Path.Combine(_storageDirectory, safeFileName);
        return File.Exists(fullPath);
    }

    public string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }

    public string GetDownloadUrl(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        return $"/api/reports/download/{safeFileName}";
    }
}
