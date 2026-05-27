using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polleria.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "OrderDate", "Type" },
                values: new object[] { new DateTime(2026, 5, 26, 20, 57, 20, 481, DateTimeKind.Utc).AddTicks(9948), 0 });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "OrderDate", "Type" },
                values: new object[] { new DateTime(2026, 5, 26, 20, 57, 20, 481, DateTimeKind.Utc).AddTicks(9961), 0 });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "OrderDate", "Type" },
                values: new object[] { new DateTime(2026, 5, 26, 20, 57, 20, 481, DateTimeKind.Utc).AddTicks(9969), 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "OrderDate",
                value: new DateTime(2026, 5, 25, 21, 7, 43, 601, DateTimeKind.Utc).AddTicks(6558));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "OrderDate",
                value: new DateTime(2026, 5, 25, 21, 7, 43, 601, DateTimeKind.Utc).AddTicks(6568));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "OrderDate",
                value: new DateTime(2026, 5, 25, 21, 7, 43, 601, DateTimeKind.Utc).AddTicks(6574));
        }
    }
}
