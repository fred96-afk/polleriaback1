using System.ComponentModel.DataAnnotations;

namespace Models.Tables;

/// <summary>
/// Solicitud para emitir una alerta genérica de mesa (por ejemplo, una incidencia).
/// </summary>
public record TableAlertRequest(
    [Required]
    [MaxLength(20)]
    string TableNumber,

    [MaxLength(250)]
    string? Message = null,

    [MaxLength(30)]
    string Type = "table_alert"
);
