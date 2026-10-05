using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class OrderSeed
{
    private static readonly DateTime SeedDate = new(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                OrderDate = SeedDate,
                ClientId = 1,
                UserId = 2,
                DeliveryUserId = 3,
                TotalAmount = 65.0m,
                TableNumber = null,
                Type = OrderType.Delivery,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending
            },
            new Order
            {
                Id = 2,
                OrderDate = SeedDate,
                ClientId = 2,
                UserId = 2,
                DeliveryUserId = null,
                TotalAmount = 35.0m,
                TableNumber = "1",
                Type = OrderType.DineIn,
                Status = OrderStatus.InPreparation,
                PaymentStatus = PaymentStatus.Pending
            },
            new Order
            {
                Id = 3,
                OrderDate = SeedDate,
                ClientId = 3,
                UserId = 2,
                DeliveryUserId = null,
                TotalAmount = 20.0m,
                TableNumber = "2",
                Type = OrderType.DineIn,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending
            }
        );
    }
}