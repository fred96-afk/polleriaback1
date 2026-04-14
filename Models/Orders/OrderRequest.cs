namespace Models.Orders;

public record OrderRequest(
    int? ClientId,
    int UserId,
    int? DeliveryUserId,
    List<OrderDetailRequest> Details,
    bool IsPos = false
);

public record OrderDetailRequest(
    int ProductId,
    int? SideId,
    int Quantity
);
