namespace IBusiness;

public interface IReportStorageService
{
    Task<string> SaveReportAsync(string fileName, byte[] data);
    Task<byte[]?> GetReportAsync(string fileName);
    Task<(byte[]? Data, string ContentType)> GetReportWithContentTypeAsync(string fileName);
    bool ReportExists(string fileName);
    string GetContentType(string fileName);
    string GetDownloadUrl(string fileName);
}
