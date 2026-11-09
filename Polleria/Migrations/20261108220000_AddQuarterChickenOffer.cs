using DbModel;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Polleria.Migrations;

[DbContext(typeof(PolleriaDbContext))]
[Migration("20261108220000_AddQuarterChickenOffer")]
public partial class AddQuarterChickenOffer : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "UPDATE [Products] SET [Name] = N'1/4 de Pollo a la Brasa', [SalePrice] = 12.00 WHERE [Id] = 3");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "UPDATE [Products] SET [Name] = N'Un Cuarto de Pollo', [SalePrice] = NULL WHERE [Id] = 3");
    }
}
