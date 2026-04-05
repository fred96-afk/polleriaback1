namespace Models.Orders;

public record OrderResponse(
    int Id,
    DateTime OrderDate,
    int? ClientId,
    int UserId,
    int? DeliveryUserId,
    decimal TotalAmount,
    List<OrderDetailResponse> Details,
    string? PaymentUrl = null
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
