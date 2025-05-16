using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HillMetrics.MIND.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ClientEntity_UserAccount_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "client_id",
                table: "user_accounts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_accounts_client_id",
                table: "user_accounts",
                column: "client_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_accounts_clients_client_id",
                table: "user_accounts",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_accounts_clients_client_id",
                table: "user_accounts");

            migrationBuilder.DropIndex(
                name: "ix_user_accounts_client_id",
                table: "user_accounts");

            migrationBuilder.DropColumn(
                name: "client_id",
                table: "user_accounts");
        }
    }
}
