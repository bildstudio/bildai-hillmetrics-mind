using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HillMetrics.MIND.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ClientEntity_IsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "use_hm_rules",
                table: "clients_flux_rules",
                newName: "use_hm_default_rules");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "clients",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "clients");

            migrationBuilder.RenameColumn(
                name: "use_hm_default_rules",
                table: "clients_flux_rules",
                newName: "use_hm_rules");
        }
    }
}
