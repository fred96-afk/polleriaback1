using IBusiness;
using Microsoft.Extensions.Logging;
using Models.Reports;

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
            if (Directory.Exists(_storageDirectory))
            {
                var matching = Directory.GetFiles(_storageDirectory)
                    .FirstOrDefault(f => string.Equals(Path.GetFileName(f), safeFileName, StringComparison.OrdinalIgnoreCase));
                if (matching != null)
                {
                    fullPath = matching;
                }
                else
                {
                    _logger.LogWarning("Reporte solicitado no encontrado en disco: {FileName}", safeFileName);
                    return null;
                }
            }
            else
            {
                return null;
            }
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
        return $"/api/Reports/download/{safeFileName}";
    }

    public Task<IEnumerable<ReportFileInfo>> GetCompletedReportsAsync()
    {
        var result = new List<ReportFileInfo>();

        if (!Directory.Exists(_storageDirectory))
        {
            return Task.FromResult<IEnumerable<ReportFileInfo>>(result);
        }

        var directoryInfo = new DirectoryInfo(_storageDirectory);
        var files = directoryInfo.GetFiles()
            .Where(f => f.Extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                        f.Extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(f => f.CreationTimeUtc);

        foreach (var file in files)
        {
            var extension = file.Extension.ToLowerInvariant();
            var reportType = extension == ".xlsx" ? "Excel" : "Pdf";

            string? jobId = null;
            var parts = Path.GetFileNameWithoutExtension(file.Name).Split('_');
            if (parts.Length >= 2 && Guid.TryParse(parts[1], out _))
            {
                jobId = parts[1];
            }

            result.Add(new ReportFileInfo
            {
                FileName = file.Name,
                DownloadUrl = GetDownloadUrl(file.Name),
                FileSize = file.Length,
                CreatedAt = file.CreationTimeUtc,
                ReportType = reportType,
                JobId = jobId
            });
        }

        return Task.FromResult<IEnumerable<ReportFileInfo>>(result);
    }

    public Task<bool> DeleteReportAsync(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        var fullPath = Path.Combine(_storageDirectory, safeFileName);

        if (File.Exists(fullPath))
        {
            try
            {
                File.Delete(fullPath);
                _logger.LogInformation("Reporte eliminado exitosamente: {FileName}", safeFileName);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar archivo de reporte: {FileName}", safeFileName);
                return Task.FromResult(false);
            }
        }

        return Task.FromResult(false);
    }
}
