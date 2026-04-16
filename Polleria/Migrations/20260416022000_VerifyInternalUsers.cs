using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polleria.Migrations
{
    public partial class VerifyInternalUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE Users
                SET IsVerified = 1,
                    VerificationToken = NULL
                WHERE RoleId IN (1, 2, 3);
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE Users
                SET IsVerified = 0
                WHERE RoleId IN (1, 2, 3);
                """);
        }
    }
}
