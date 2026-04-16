using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polleria.Migrations
{
    public partial class CrearStoredProcedureReportes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE OR ALTER PROCEDURE dbo.sp_GetOrdersByDateRange
                    @StartDate DATETIME2,
                    @EndDateExclusive DATETIME2
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT
                        Id,
                        OrderDate,
                        ClientId,
                        UserId,
                        DeliveryUserId,
                        TotalAmount,
                        Status,
                        PaymentStatus
                    FROM Orders
                    WHERE OrderDate >= @StartDate
                      AND OrderDate < @EndDateExclusive
                    ORDER BY OrderDate DESC;
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'dbo.sp_GetOrdersByDateRange', N'P') IS NOT NULL
                    DROP PROCEDURE dbo.sp_GetOrdersByDateRange;
                """);
        }
    }
}
