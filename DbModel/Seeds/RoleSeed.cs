using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class RoleSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Waiter" },
            new Role { Id = 3, Name = "Delivery" },
            new Role { Id = 4, Name = "Client" },
            new Role { Id = 5, Name = "Cashier" }
        );
    }
}