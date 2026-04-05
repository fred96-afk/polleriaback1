using System.ComponentModel.DataAnnotations;

namespace DbModel.Tables;

public class Order
{
    [Key]
    public int Id { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public int? ClientId { get; set; }

    public int UserId { get; set; } // The employee who took the order (Mozo/Admin)

    public int? DeliveryUserId { get; set; } // The employee who delivers (Delivery)

    public decimal TotalAmount { get; set; }
}