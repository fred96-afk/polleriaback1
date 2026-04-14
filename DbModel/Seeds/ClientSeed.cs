using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class ClientSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>().HasData(
            new Client { Id = 1, Name = "Juan Perez", Phone = "987654321", DocumentType = "DNI", DocumentNumber = "12345678", Address = "Av. Siempre Viva 123" },
            new Client { Id = 2, Name = "Maria Garcia", Phone = "912345678", DocumentType = "DNI", DocumentNumber = "87654321", Address = "Calle Real 456" },
            new Client { Id = 3, Name = "Carlos Sanchez", Phone = "954321098", DocumentType = "RUC", DocumentNumber = "20123456789", Address = "Urb. Los Rosales 789" }
        );
    }
}
