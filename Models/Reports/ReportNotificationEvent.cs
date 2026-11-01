using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Models.Reports;

public class ReportNotificationEvent
{
    [JsonProperty("jobId")]
    [JsonPropertyName("jobId")]
    public string JobId { get; set; } = string.Empty;

    [JsonProperty("reportType")]
    [JsonPropertyName("reportType")]
    public string ReportType { get; set; } = string.Empty; // "Excel" or "Pdf"

    [JsonProperty("status")]
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;     // "Queued", "Processing", "Completed", "Failed"

    [JsonProperty("message")]
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("fileName")]
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }

    [JsonProperty("downloadUrl")]
    [JsonPropertyName("downloadUrl")]
    public string? DownloadUrl { get; set; }

    [JsonProperty("fileSize")]
    [JsonPropertyName("fileSize")]
    public long? FileSize { get; set; }

    [JsonProperty("timestamp")]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonProperty("error")]
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
