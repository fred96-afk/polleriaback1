using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polleria.Migrations
{
    /// <inheritdoc />
    public partial class documentstype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2026, 4, 13, 21, 20, 8, 431, DateTimeKind.Utc).AddTicks(3474));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "OrderDate",
                value: new DateTime(2026, 4, 13, 21, 20, 8, 431, DateTimeKind.Utc).AddTicks(3485));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "OrderDate",
                value: new DateTime(2026, 4, 13, 21, 20, 8, 431, DateTimeKind.Utc).AddTicks(3490));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2026, 4, 13, 20, 8, 13, 963, DateTimeKind.Utc).AddTicks(1893));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "OrderDate",
                value: new DateTime(2026, 4, 13, 20, 8, 13, 963, DateTimeKind.Utc).AddTicks(1902));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "OrderDate",
                value: new DateTime(2026, 4, 13, 20, 8, 13, 963, DateTimeKind.Utc).AddTicks(1909));
        }
    }
}
