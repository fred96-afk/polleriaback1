using System.ComponentModel.DataAnnotations;

namespace Models.Tables;

/// <summary>
/// Solicitud para emitir una notificación en tiempo real asociada a una mesa.
/// </summary>
public record TableNotificationRequest(
    [Required]
    [MaxLength(20)]
    string TableNumber,

    [MaxLength(250)]
    string? Message = null
);
