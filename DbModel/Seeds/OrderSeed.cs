using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class OrderSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, OrderDate = PeruTimeHelper.Now, ClientId = 1, UserId = 2, DeliveryUserId = 3, TotalAmount = 65.0m },
            new Order { Id = 2, OrderDate = PeruTimeHelper.Now, ClientId = 2, UserId = 2, TotalAmount = 35.0m },
            new Order { Id = 3, OrderDate = PeruTimeHelper.Now, ClientId = 3, UserId = 1, TotalAmount = 20.0m }
        );
    }
}