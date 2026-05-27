namespace Models.Orders;

public record OrderResponse(
    int Id,
    DateTime OrderDate,
    int? ClientId,
    string? CustomerName,
    string? CustomerAddress,
    string? CustomerPhone,
    string? CustomerEmail,
    int UserId,
    string? EmployeeName,
    int? DeliveryUserId,
    string? DeliveryName,
    decimal TotalAmount,
    string Type,
    List<OrderDetailResponse> Details,
    string Status = "Pending",
    string PaymentStatus = "Pending",
    string? PaymentUrl = null,
    string? PdfUrl = null
);

public record OrderDetailResponse(
    int Id,
    int ProductId,
    string? ProductName,
    int? SideId,
    string? SideName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal
);
