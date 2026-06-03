using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class PermissionSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = "Dashboard", Code = "dashboard.view" },
            new Permission { Id = 2, Name = "Ver Pedidos", Code = "orders.view" },
            new Permission { Id = 3, Name = "Crear Pedidos", Code = "orders.create" },
            new Permission { Id = 4, Name = "Editar Pedidos", Code = "orders.edit" },
            new Permission { Id = 5, Name = "Gestionar Usuarios", Code = "users.manage" },
            new Permission { Id = 6, Name = "Gestionar Roles", Code = "roles.manage" },
            new Permission { Id = 7, Name = "Ver Reportes", Code = "reports.view" },
            new Permission { Id = 8, Name = "Gestionar Catálogo", Code = "catalog.manage" },
            new Permission { Id = 9, Name = "Gestionar Pagos", Code = "orders.pay" }
        );
    }
}