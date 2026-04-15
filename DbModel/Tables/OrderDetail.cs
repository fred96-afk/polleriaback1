using System.ComponentModel.DataAnnotations;

namespace DbModel.Tables;

public class OrderDetail
{
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }
    public virtual Order? Order { get; set; }

    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }

    public int? SideId { get; set; } // Extra side choice
    public virtual Side? Side { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }
}