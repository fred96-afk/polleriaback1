namespace DbModel.Tables;

public enum OrderStatus
{
    Pending,
    Accepted,
    InPreparation,
    Ready,
    OnTheWay,
    Delivered,
    Cancelled
}

public enum PaymentStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled
}

public enum OrderType
{
    Delivery,
    Pickup,
    POS,
    DineIn
}
