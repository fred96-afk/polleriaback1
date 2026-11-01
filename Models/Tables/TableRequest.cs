using System.ComponentModel.DataAnnotations;

namespace Models.Tables;

public record TableRequest(
    [Required]
    [MaxLength(20)]
    string Number,

    [MaxLength(100)]
    string? Name = null,

    [Range(1, 50)]
    int Chairs = 4,

    [MaxLength(100)]
    string Location = "Salón Principal",

    [MaxLength(50)]
    string MapId = "salon-principal",

    [MaxLength(20)]
    string Shape = "square",

    double X = 20.0,
    double Y = 20.0,
    bool IsActive = true
);

public record TablePositionRequest(
    [Range(0, 100)]
    double X,

    [Range(0, 100)]
    double Y
);
