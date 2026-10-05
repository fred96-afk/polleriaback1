using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class PaymentSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>().HasData(
            new Payment { Id = 1, TransactionId = "TXN001", Status = "Paid", PaymentMethod = "Cash", Amount = 65.0m, OrderId = 1 },
            new Payment { Id = 2, TransactionId = "TXN002", Status = "Paid", PaymentMethod = "Card", Amount = 35.0m, OrderId = 2 },
            new Payment { Id = 3, TransactionId = "TXN003", Status = "Paid", PaymentMethod = "Yape", Amount = 20.0m, OrderId = 3 }
        );
    }
}