using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Models.Reports;

public class ReportFileInfo
{
    [JsonProperty("fileName")]
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    [JsonProperty("downloadUrl")]
    [JsonPropertyName("downloadUrl")]
    public string DownloadUrl { get; set; } = string.Empty;

    [JsonProperty("fileSize")]
    [JsonPropertyName("fileSize")]
    public long FileSize { get; set; }

    [JsonProperty("createdAt")]
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("reportType")]
    [JsonPropertyName("reportType")]
    public string ReportType { get; set; } = string.Empty; // "Excel" or "Pdf"

    [JsonProperty("jobId")]
    [JsonPropertyName("jobId")]
    public string? JobId { get; set; }
}
