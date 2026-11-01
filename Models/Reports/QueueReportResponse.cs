using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Models.Reports;

public class QueueReportResponse
{
    [JsonProperty("jobId")]
    [JsonPropertyName("jobId")]
    public string JobId { get; set; } = string.Empty;

    [JsonProperty("reportType")]
    [JsonPropertyName("reportType")]
    public string ReportType { get; set; } = string.Empty;

    [JsonProperty("status")]
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("channel")]
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }

    [JsonProperty("message")]
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("requestedAt")]
    [JsonPropertyName("requestedAt")]
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}
