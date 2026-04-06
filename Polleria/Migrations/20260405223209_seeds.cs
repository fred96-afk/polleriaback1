using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Polleria.Migrations
{
    /// <inheritdoc />
    public partial class seeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Banners",
                columns: new[] { "Id", "ImageUrl", "IsActive", "LinkUrl", "Order", "Subtitle", "Title" },
                values: new object[,]
                {
                    { 1, "https://example.com/banner1.jpg", true, null, 1, "Prueba el mejor pollo a la brasa", "¡Bienvenido!" },
                    { 2, "https://example.com/banner2.jpg", true, null, 2, "2 Pollos x 110 soles", "Oferta del Mes" },
                    { 3, "https://example.com/banner3.jpg", true, null, 3, "Solo en zonas seleccionadas", "Delivery Gratis" }
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Address", "Name", "Phone" },
                values: new object[,]
                {
                    { 1, "Av. Siempre Viva 123", "Juan Perez", "987654321" },
                    { 2, "Calle Real 456", "Maria Garcia", "912345678" },
                    { 3, "Urb. Los Rosales 789", "Carlos Sanchez", "954321098" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "ClientId", "DeliveryUserId", "OrderDate", "TotalAmount", "UserId" },
                values: new object[,]
                {
                    { 1, 1, 3, new DateTime(2026, 4, 5, 22, 32, 6, 602, DateTimeKind.Utc).AddTicks(3166), 65.0m, 2 },
                    { 2, 2, null, new DateTime(2026, 4, 5, 22, 32, 6, 602, DateTimeKind.Utc).AddTicks(3178), 35.0m, 2 },
                    { 3, 3, null, new DateTime(2026, 4, 5, 22, 32, 6, 602, DateTimeKind.Utc).AddTicks(3186), 20.0m, 1 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BasePrice", "Description", "ImageUrl", "Name", "SalePrice" },
                values: new object[,]
                {
                    { 1, 65.0m, "Pollo entero con papas y ensalada", "https://example.com/pollo_entero.jpg", "Pollo a la Brasa (Entero)", 60.0m },
                    { 2, 35.0m, "Medio pollo con papas y ensalada", "https://example.com/medio_pollo.jpg", "Medio Pollo", null },
                    { 3, 20.0m, "Un cuarto de pollo con papas y ensalada", "https://example.com/cuarto_pollo.jpg", "Un Cuarto de Pollo", null }
                });

            migrationBuilder.InsertData(
                table: "Sides",
                columns: new[] { "Id", "Description", "Name", "Price", "Type" },
                values: new object[] { 3, null, "Papas Fritas", 8.0m, 1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "PasswordHash", "RoleId" },
                values: new object[,]
                {
                    { 1, "admin@example.com", "Admin", "AQAAAAIAAYagAAAAEP0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O", 1 },
                    { 2, "mozo1@example.com", "Mozo 1", "AQAAAAIAAYagAAAAEP0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O", 2 },
                    { 3, "delivery1@example.com", "Delivery 1", "AQAAAAIAAYagAAAAEP0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O0O", 3 }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "Id", "CdrUrl", "ExternalId", "Number", "OrderId", "PdfUrl", "Serie", "XmlUrl" },
                values: new object[,]
                {
                    { 1, null, "EXT001", 1, 1, null, "F001", null },
                    { 2, null, "EXT002", 2, 2, null, "F001", null },
                    { 3, null, "EXT003", 3, 3, null, "F001", null }
                });

            migrationBuilder.InsertData(
                table: "OrderDetails",
                columns: new[] { "Id", "OrderId", "ProductId", "Quantity", "SideId", "Subtotal", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, 3, 65.0m, 65.0m },
                    { 2, 2, 2, 1, 1, 35.0m, 35.0m },
                    { 3, 3, 3, 1, 2, 20.0m, 20.0m }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "OrderId", "PaymentMethod", "Status", "TransactionId" },
                values: new object[,]
                {
                    { 1, 65.0m, 1, "Cash", "Paid", "TXN001" },
                    { 2, 35.0m, 2, "Card", "Paid", "TXN002" },
                    { 3, 20.0m, 3, "Yape", "Paid", "TXN003" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Banners",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Banners",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Banners",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Sides",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
