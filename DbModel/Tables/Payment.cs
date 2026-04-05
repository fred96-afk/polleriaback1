using System.ComponentModel.DataAnnotations;

namespace DbModel.Tables;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string TransactionId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? PaymentMethod { get; set; }

    public decimal Amount { get; set; }

    public int OrderId { get; set; }
}
