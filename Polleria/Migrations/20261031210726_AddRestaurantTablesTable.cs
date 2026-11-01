using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Polleria.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantTablesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Chairs = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MapId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Shape = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    X = table.Column<double>(type: "float", nullable: false),
                    Y = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "Chairs", "CreatedAt", "IsActive", "Location", "MapId", "Name", "Number", "Shape", "UpdatedAt", "X", "Y" },
                values: new object[,]
                {
                    { 1, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Frente a Entrada", "salon-principal", "Mesa 1", "1", "square", null, 8.0, 12.0 },
                    { 2, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Zona Centro", "salon-principal", "Mesa 2", "2", "square", null, 32.0, 12.0 },
                    { 3, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Zona Familiar", "salon-principal", "Mesa 3", "3", "rectangle", null, 56.0, 12.0 },
                    { 4, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Zona Familiar", "salon-principal", "Mesa 4", "4", "rectangle", null, 80.0, 12.0 },
                    { 5, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Pasillo Central", "salon-principal", "Mesa 5", "5", "round", null, 8.0, 44.0 },
                    { 6, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Pasillo Central", "salon-principal", "Mesa 6", "6", "round", null, 32.0, 44.0 },
                    { 7, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Mesa Grupal", "salon-principal", "Mesa 7", "7", "rectangle", null, 56.0, 44.0 },
                    { 8, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Pasillo Central", "salon-principal", "Mesa 8", "8", "round", null, 80.0, 44.0 },
                    { 9, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Parejas Ventana", "salon-principal", "Mesa 9", "9", "round", null, 8.0, 76.0 },
                    { 10, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Parejas Ventana", "salon-principal", "Mesa 10", "10", "round", null, 32.0, 76.0 },
                    { 11, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Junto a Barra", "salon-principal", "Mesa 11", "11", "square", null, 56.0, 76.0 },
                    { 12, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Salón Principal - Junto a Barra", "salon-principal", "Mesa 12", "12", "square", null, 80.0, 76.0 },
                    { 13, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Terraza Exterior - Vista Avenida", "terraza", "Mesa Terraza 1", "T-1", "round", null, 15.0, 20.0 },
                    { 14, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Terraza Exterior - Vista Avenida", "terraza", "Mesa Terraza 2", "T-2", "round", null, 45.0, 20.0 },
                    { 15, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Terraza Exterior - Área Jardín", "terraza", "Mesa Terraza 3", "T-3", "rectangle", null, 75.0, 20.0 },
                    { 16, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Terraza Exterior - Zona Sombrillas", "terraza", "Mesa Terraza 4", "T-4", "round", null, 15.0, 65.0 },
                    { 17, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Terraza Exterior - Zona Sombrillas", "terraza", "Mesa Terraza 5", "T-5", "round", null, 45.0, 65.0 },
                    { 18, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Terraza Exterior - Área Jardín", "terraza", "Mesa Terraza 6", "T-6", "rectangle", null, 75.0, 65.0 },
                    { 19, 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Segundo Piso - Salón Presidencial", "segundo-piso", "Mesa Presidencial", "VIP-1", "rectangle", null, 25.0, 30.0 },
                    { 20, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Segundo Piso - Sala de Eventos", "segundo-piso", "Mesa Corporativa", "VIP-2", "rectangle", null, 70.0, 30.0 },
                    { 21, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Segundo Piso - Balcón Panorámico A", "segundo-piso", "Salón Balcón A", "VIP-3", "round", null, 25.0, 70.0 },
                    { 22, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Segundo Piso - Balcón Panorámico B", "segundo-piso", "Salón Balcón B", "VIP-4", "round", null, 70.0, 70.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tables");
        }
    }
}
