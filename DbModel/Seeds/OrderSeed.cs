using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class OrderSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, OrderDate = DateTime.UtcNow, ClientId = 1, UserId = 2, DeliveryUserId = 3, TotalAmount = 65.0m },
            new Order { Id = 2, OrderDate = DateTime.UtcNow, ClientId = 2, UserId = 2, TotalAmount = 35.0m },
            new Order { Id = 3, OrderDate = DateTime.UtcNow, ClientId = 3, UserId = 1, TotalAmount = 20.0m }
        );
    }
}