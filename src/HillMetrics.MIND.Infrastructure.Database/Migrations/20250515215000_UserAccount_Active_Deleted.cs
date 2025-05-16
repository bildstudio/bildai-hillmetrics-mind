using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HillMetrics.MIND.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UserAccount_Active_Deleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "user_accounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "user_accounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "user_accounts");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "user_accounts");
        }
    }
}
