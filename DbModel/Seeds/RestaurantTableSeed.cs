using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class RestaurantTableSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RestaurantTable>().HasData(
            // Salón Principal
            new RestaurantTable { Id = 1, Number = "1", Name = "Mesa 1", Chairs = 4, Location = "Salón Principal - Frente a Entrada", MapId = "salon-principal", Shape = "square", X = 8, Y = 12, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 2, Number = "2", Name = "Mesa 2", Chairs = 4, Location = "Salón Principal - Zona Centro", MapId = "salon-principal", Shape = "square", X = 32, Y = 12, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 3, Number = "3", Name = "Mesa 3", Chairs = 6, Location = "Salón Principal - Zona Familiar", MapId = "salon-principal", Shape = "rectangle", X = 56, Y = 12, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 4, Number = "4", Name = "Mesa 4", Chairs = 6, Location = "Salón Principal - Zona Familiar", MapId = "salon-principal", Shape = "rectangle", X = 80, Y = 12, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 5, Number = "5", Name = "Mesa 5", Chairs = 4, Location = "Salón Principal - Pasillo Central", MapId = "salon-principal", Shape = "round", X = 8, Y = 44, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 6, Number = "6", Name = "Mesa 6", Chairs = 4, Location = "Salón Principal - Pasillo Central", MapId = "salon-principal", Shape = "round", X = 32, Y = 44, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 7, Number = "7", Name = "Mesa 7", Chairs = 8, Location = "Salón Principal - Mesa Grupal", MapId = "salon-principal", Shape = "rectangle", X = 56, Y = 44, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 8, Number = "8", Name = "Mesa 8", Chairs = 4, Location = "Salón Principal - Pasillo Central", MapId = "salon-principal", Shape = "round", X = 80, Y = 44, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 9, Number = "9", Name = "Mesa 9", Chairs = 2, Location = "Salón Principal - Parejas Ventana", MapId = "salon-principal", Shape = "round", X = 8, Y = 76, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 10, Number = "10", Name = "Mesa 10", Chairs = 2, Location = "Salón Principal - Parejas Ventana", MapId = "salon-principal", Shape = "round", X = 32, Y = 76, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 11, Number = "11", Name = "Mesa 11", Chairs = 4, Location = "Salón Principal - Junto a Barra", MapId = "salon-principal", Shape = "square", X = 56, Y = 76, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 12, Number = "12", Name = "Mesa 12", Chairs = 4, Location = "Salón Principal - Junto a Barra", MapId = "salon-principal", Shape = "square", X = 80, Y = 76, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // Terraza Exterior
            new RestaurantTable { Id = 13, Number = "T-1", Name = "Mesa Terraza 1", Chairs = 4, Location = "Terraza Exterior - Vista Avenida", MapId = "terraza", Shape = "round", X = 15, Y = 20, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 14, Number = "T-2", Name = "Mesa Terraza 2", Chairs = 4, Location = "Terraza Exterior - Vista Avenida", MapId = "terraza", Shape = "round", X = 45, Y = 20, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 15, Number = "T-3", Name = "Mesa Terraza 3", Chairs = 6, Location = "Terraza Exterior - Área Jardín", MapId = "terraza", Shape = "rectangle", X = 75, Y = 20, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 16, Number = "T-4", Name = "Mesa Terraza 4", Chairs = 4, Location = "Terraza Exterior - Zona Sombrillas", MapId = "terraza", Shape = "round", X = 15, Y = 65, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 17, Number = "T-5", Name = "Mesa Terraza 5", Chairs = 4, Location = "Terraza Exterior - Zona Sombrillas", MapId = "terraza", Shape = "round", X = 45, Y = 65, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 18, Number = "T-6", Name = "Mesa Terraza 6", Chairs = 6, Location = "Terraza Exterior - Área Jardín", MapId = "terraza", Shape = "rectangle", X = 75, Y = 65, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // Segundo Nivel VIP
            new RestaurantTable { Id = 19, Number = "VIP-1", Name = "Mesa Presidencial", Chairs = 10, Location = "Segundo Piso - Salón Presidencial", MapId = "segundo-piso", Shape = "rectangle", X = 25, Y = 30, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 20, Number = "VIP-2", Name = "Mesa Corporativa", Chairs = 8, Location = "Segundo Piso - Sala de Eventos", MapId = "segundo-piso", Shape = "rectangle", X = 70, Y = 30, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 21, Number = "VIP-3", Name = "Salón Balcón A", Chairs = 4, Location = "Segundo Piso - Balcón Panorámico A", MapId = "segundo-piso", Shape = "round", X = 25, Y = 70, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new RestaurantTable { Id = 22, Number = "VIP-4", Name = "Salón Balcón B", Chairs = 4, Location = "Segundo Piso - Balcón Panorámico B", MapId = "segundo-piso", Shape = "round", X = 70, Y = 70, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
