namespace Models.Reports;

public class ReportNotificationEvent
{
    public string JobId { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty; // "Excel" or "Pdf"
    public string Status { get; set; } = string.Empty;     // "Queued", "Processing", "Completed", "Failed"
    public string Message { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? DownloadUrl { get; set; }
    public long? FileSize { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Error { get; set; }
}
