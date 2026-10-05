using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class ProductSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Pollo a la Brasa (Entero)", Description = "Pollo entero con papas y ensalada", BasePrice = 65.0m, SalePrice = 60.0m, ImageUrl = "https://example.com/pollo_entero.jpg" },
            new Product { Id = 2, Name = "Medio Pollo", Description = "Medio pollo con papas y ensalada", BasePrice = 35.0m, ImageUrl = "https://example.com/medio_pollo.jpg" },
            new Product { Id = 3, Name = "Un Cuarto de Pollo", Description = "Un cuarto de pollo con papas y ensalada", BasePrice = 20.0m, ImageUrl = "https://example.com/cuarto_pollo.jpg" }
        );
    }
}