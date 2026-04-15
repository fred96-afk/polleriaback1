namespace Models.Orders;

public record OrderRequest(
    int? ClientId,
    int UserId,
    int? DeliveryUserId,
    List<OrderDetailRequest> Details,
    bool IsPos = false,
    string? CustomerName = null,
    string? DocumentNumber = null,
    string? DocumentType = "DNI",
    string? CustomerEmail = null
);

public record OrderDetailRequest(
    int ProductId,
    int? SideId,
    int Quantity
);
