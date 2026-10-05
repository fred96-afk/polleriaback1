using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class SideSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Side>().HasData(
            new Side { Id = 1, Name = "Ensalada Dulce", Type = SideType.Sweet, Price = 5.0m },
            new Side { Id = 2, Name = "Ensalada Salada", Type = SideType.Savory, Price = 5.0m },
            new Side { Id = 3, Name = "Papas Fritas", Type = SideType.Savory, Price = 8.0m }
        );
    }
}