using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polleria.Migrations
{
    /// <inheritdoc />
    public partial class SeedOrdersWithTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2026, 6, 1, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "OrderDate", "Status", "TableNumber", "Type" },
                values: new object[] { new DateTime(2026, 6, 1, 12, 0, 0, 0, DateTimeKind.Utc), 2, "1", 3 });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "OrderDate", "TableNumber", "Type", "UserId" },
                values: new object[] { new DateTime(2026, 6, 1, 12, 0, 0, 0, DateTimeKind.Utc), "2", 3, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                columns: new[] { "OrderDate", "Status", "TableNumber", "Type" },
                values: new object[] { new DateTime(2026, 10, 2, 19, 2, 49, 795, DateTimeKind.Unspecified).AddTicks(6218), 0, null, 0 });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "OrderDate", "TableNumber", "Type", "UserId" },
                values: new object[] { new DateTime(2026, 10, 2, 19, 2, 49, 795, DateTimeKind.Unspecified).AddTicks(6231), null, 0, 1 });
        }
    }
}
