namespace Models.Reports;

public class QueueReportResponse
{
    public string JobId { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Channel { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}
