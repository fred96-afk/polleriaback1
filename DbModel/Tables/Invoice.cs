using System.ComponentModel.DataAnnotations;

namespace DbModel.Tables;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Serie { get; set; } = string.Empty;

    [Required]
    public int Number { get; set; }

    [Required]
    [MaxLength(50)]
    public string ExternalId { get; set; } = string.Empty;

    public string? PdfUrl { get; set; }
    public string? XmlUrl { get; set; }
    public string? CdrUrl { get; set; }

    public int OrderId { get; set; }
}
