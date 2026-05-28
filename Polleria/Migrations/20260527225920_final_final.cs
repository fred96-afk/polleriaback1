using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polleria.Migrations
{
    /// <inheritdoc />
    public partial class final_final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2026, 5, 26, 20, 57, 20, 481, DateTimeKind.Utc).AddTicks(9948));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "OrderDate",
                value: new DateTime(2026, 5, 26, 20, 57, 20, 481, DateTimeKind.Utc).AddTicks(9961));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "OrderDate",
                value: new DateTime(2026, 5, 26, 20, 57, 20, 481, DateTimeKind.Utc).AddTicks(9969));
        }
    }
}
