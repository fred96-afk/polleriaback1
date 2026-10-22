namespace Models.Reports;

public class ReportQueueMessage
{
    public string JobId { get; set; } = Guid.NewGuid().ToString();
    public string ReportType { get; set; } = string.Empty; // "Excel" or "Pdf"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Channel { get; set; } = "reports";
    public string? UserId { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}
