using Microsoft.EntityFrameworkCore;
using DbModel.Tables;

namespace DbModel.Seeds;

public static class RolePermissionSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolePermission>().HasData(
            // Admin gets everything
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 1, PermissionId = 3 },
            new RolePermission { RoleId = 1, PermissionId = 4 },
            new RolePermission { RoleId = 1, PermissionId = 5 },
            new RolePermission { RoleId = 1, PermissionId = 6 },
            new RolePermission { RoleId = 1, PermissionId = 7 },
            new RolePermission { RoleId = 1, PermissionId = 8 },
            new RolePermission { RoleId = 1, PermissionId = 9 },

            // Waiter
            new RolePermission { RoleId = 2, PermissionId = 2 }, // Ver pedidos
            new RolePermission { RoleId = 2, PermissionId = 3 }, // Crear pedidos
            new RolePermission { RoleId = 2, PermissionId = 4 }, // Editar pedidos

            // Delivery
            new RolePermission { RoleId = 3, PermissionId = 2 }, // Ver pedidos

            // Cashier (Cajero)
            new RolePermission { RoleId = 5, PermissionId = 1 }, // Dashboard
            new RolePermission { RoleId = 5, PermissionId = 2 }, // Ver pedidos
            new RolePermission { RoleId = 5, PermissionId = 7 }, // Ver reportes
            new RolePermission { RoleId = 5, PermissionId = 9 }  // Gestionar pagos
        );
    }
}