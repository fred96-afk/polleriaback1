using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class RoleSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Administrador" },
            new Role { Id = 2, Name = "Mozo" },
            new Role { Id = 3, Name = "Delivery" }
        );
    }
}