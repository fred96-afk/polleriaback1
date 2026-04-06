using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class ClientSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>().HasData(
            new Client { Id = 1, Name = "Juan Perez", Phone = "987654321", Address = "Av. Siempre Viva 123" },
            new Client { Id = 2, Name = "Maria Garcia", Phone = "912345678", Address = "Calle Real 456" },
            new Client { Id = 3, Name = "Carlos Sanchez", Phone = "954321098", Address = "Urb. Los Rosales 789" }
        );
    }
}