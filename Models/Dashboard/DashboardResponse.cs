namespace Models.Dashboard;

public record DashboardResponse(
    decimal TotalRevenue,
    int TotalOrders,
    int PendingOrders,
    int TotalProducts,
    List<TopProductResponse> TopProducts,
    List<SalesByDayResponse> SalesLast7Days,
    List<RecentOrderResponse> RecentOrders
);

public record TopProductResponse(
    string ProductName,
    int QuantitySold
);

public record SalesByDayResponse(
    string Day,
    decimal Total
);

public record RecentOrderResponse(
    int Id,
    string ClientName,
    decimal Total,
    DateTime Date
);
