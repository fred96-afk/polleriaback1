using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Models.Tables;

/// <summary>
/// Payload enviado por Pusher a los clientes suscritos al canal "tables".
/// Los nombres de las propiedades se serializan en camelCase para que coincidan
/// con lo esperado por la app móvil (Flutter) y el frontend (Angular).
/// </summary>
public class TableNotificationEvent
{
    [JsonProperty("tableId")]
    [JsonPropertyName("tableId")]
    public int TableId { get; set; }

    [JsonProperty("tableNumber")]
    [JsonPropertyName("tableNumber")]
    public string TableNumber { get; set; } = string.Empty;

    [JsonProperty("tableName")]
    [JsonPropertyName("tableName")]
    public string? TableName { get; set; }

    [JsonProperty("type")]
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("message")]
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("timestamp")]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
