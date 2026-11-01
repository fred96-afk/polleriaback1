using System.ComponentModel.DataAnnotations;

namespace DbModel.Tables;

public class RestaurantTable
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Number { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Número de sillas asignadas a la mesa (capacidad de comensales).
    /// </summary>
    [Required]
    [Range(1, 50)]
    public int Chairs { get; set; } = 4;

    /// <summary>
    /// Ubicación física o sector dentro del local (ej. Salón Principal, Terraza, Balcón, Segundo Piso).
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Location { get; set; } = "Salón Principal";

    /// <summary>
    /// Identificador del plano o mapa 2D del local al que pertenece.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string MapId { get; set; } = "salon-principal";

    /// <summary>
    /// Forma visual de la mesa en el mapa: "square", "round", "rectangle".
    /// </summary>
    [MaxLength(20)]
    public string Shape { get; set; } = "square";

    /// <summary>
    /// Posición porcentual horizontal X en el mapa del local (0 - 100%).
    /// </summary>
    public double X { get; set; } = 20.0;

    /// <summary>
    /// Posición porcentual vertical Y en el mapa del local (0 - 100%).
    /// </summary>
    public double Y { get; set; } = 20.0;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
