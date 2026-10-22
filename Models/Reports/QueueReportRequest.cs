namespace Models.Reports;

public class QueueReportRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? ReportType { get; set; } // "Excel" or "Pdf"
    public string? Channel { get; set; } = "reports";
    public string? UserId { get; set; }
}
