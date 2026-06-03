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

    public string? TableNumber { get; set; }

    public OrderType Type { get; set; } = OrderType.Delivery;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    // Navigation properties
    public virtual Client? Client { get; set; }
    public virtual User? User { get; set; }
    public virtual User? DeliveryUser { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}