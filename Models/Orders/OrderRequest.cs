namespace Models.Orders;

public record OrderRequest(
    int? ClientId,
    int UserId,
    int? DeliveryUserId,
    List<OrderDetailRequest> Details
);

public record OrderDetailRequest(
    int ProductId,
    int? SideId,
    int Quantity
);
