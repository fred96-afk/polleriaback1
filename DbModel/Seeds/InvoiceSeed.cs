using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class InvoiceSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>().HasData(
            new Invoice { Id = 1, Serie = "F001", Number = 1, ExternalId = "EXT001", OrderId = 1 },
            new Invoice { Id = 2, Serie = "F001", Number = 2, ExternalId = "EXT002", OrderId = 2 },
            new Invoice { Id = 3, Serie = "F001", Number = 3, ExternalId = "EXT003", OrderId = 3 }
        );
    }
}