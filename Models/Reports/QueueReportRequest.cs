using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Models.Reports;

public class QueueReportRequest
{
    [JsonProperty("startDate")]
    [JsonPropertyName("startDate")]
    public DateTime? StartDate { get; set; }

    [JsonProperty("endDate")]
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    [JsonProperty("reportType")]
    [JsonPropertyName("reportType")]
    public string? ReportType { get; set; } // "Excel" or "Pdf"

    [JsonProperty("channel")]
    [JsonPropertyName("channel")]
    public string? Channel { get; set; } = "reports";

    [JsonProperty("userId")]
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
}
