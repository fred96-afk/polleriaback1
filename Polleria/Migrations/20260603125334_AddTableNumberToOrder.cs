using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polleria.Migrations
{
    /// <inheritdoc />
    public partial class AddTableNumberToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TableNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "OrderDate", "TableNumber" },
                values: new object[] { new DateTime(2026, 6, 3, 12, 53, 32, 600, DateTimeKind.Utc).AddTicks(6746), null });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "OrderDate", "TableNumber" },
                values: new object[] { new DateTime(2026, 6, 3, 12, 53, 32, 600, DateTimeKind.Utc).AddTicks(6752), null });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "OrderDate", "TableNumber" },
                values: new object[] { new DateTime(2026, 6, 3, 12, 53, 32, 600, DateTimeKind.Utc).AddTicks(6755), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableNumber",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2026, 5, 27, 22, 59, 9, 393, DateTimeKind.Utc).AddTicks(4444));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "OrderDate",
                value: new DateTime(2026, 5, 27, 22, 59, 9, 393, DateTimeKind.Utc).AddTicks(4460));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "OrderDate",
                value: new DateTime(2026, 5, 27, 22, 59, 9, 393, DateTimeKind.Utc).AddTicks(4467));
        }
    }
}
