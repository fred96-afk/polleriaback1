using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class UserSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Admin", Email = "admin@empresa.com", PasswordHash = "$2a$11$g9p.pmoBRECs8uL3nf0ns.ljh.I/VXgF9Mw/aT6LeVEGfz8oQ/QYG", RoleId = 1 },
            new User { Id = 2, Name = "Mozo 1", Email = "mozo1@example.com", PasswordHash = "AQAAAAIAAYagAAAAEP0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O", RoleId = 2 },
            new User { Id = 3, Name = "Delivery 1", Email = "delivery1@example.com", PasswordHash = "AQAAAAIAAYagAAAAEP0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O", RoleId = 3 }
        );
    }
}