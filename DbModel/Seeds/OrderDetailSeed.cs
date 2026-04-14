using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class OrderDetailSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderDetail>().HasData(
            new OrderDetail { Id = 1, OrderId = 1, ProductId = 1, SideId = 3, Quantity = 1, UnitPrice = 65.0m, Subtotal = 65.0m },
            new OrderDetail { Id = 2, OrderId = 2, ProductId = 2, SideId = 1, Quantity = 1, UnitPrice = 35.0m, Subtotal = 35.0m },
            new OrderDetail { Id = 3, OrderId = 3, ProductId = 3, SideId = 2, Quantity = 1, UnitPrice = 20.0m, Subtotal = 20.0m }
        );
    }
}