using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class BannerSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Banner>().HasData(
            new Banner { Id = 1, Title = "¡Bienvenido!", Subtitle = "Prueba el mejor pollo a la brasa", ImageUrl = "https://example.com/banner1.jpg", IsActive = true, Order = 1 },
            new Banner { Id = 2, Title = "Oferta del Mes", Subtitle = "2 Pollos x 110 soles", ImageUrl = "https://example.com/banner2.jpg", IsActive = true, Order = 2 },
            new Banner { Id = 3, Title = "Delivery Gratis", Subtitle = "Solo en zonas seleccionadas", ImageUrl = "https://example.com/banner3.jpg", IsActive = true, Order = 3 }
        );
    }
}