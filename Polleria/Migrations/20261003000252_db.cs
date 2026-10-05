using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polleria.Migrations
{
    /// <inheritdoc />
    public partial class db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2026, 10, 2, 19, 2, 49, 794, DateTimeKind.Unspecified).AddTicks(9866));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "OrderDate",
                value: new DateTime(2026, 10, 2, 19, 2, 49, 795, DateTimeKind.Unspecified).AddTicks(6218));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "OrderDate",
                value: new DateTime(2026, 10, 2, 19, 2, 49, 795, DateTimeKind.Unspecified).AddTicks(6231));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2026, 6, 3, 20, 27, 35, 357, DateTimeKind.Utc).AddTicks(6385));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "OrderDate",
                value: new DateTime(2026, 6, 3, 20, 27, 35, 357, DateTimeKind.Utc).AddTicks(6398));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "OrderDate",
                value: new DateTime(2026, 6, 3, 20, 27, 35, 357, DateTimeKind.Utc).AddTicks(6402));
        }
    }
}
