using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HillMetrics.MIND.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class PeerGroup_ClientFluxRuleEntityRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_clients_flux_rules_peer_group_id",
                table: "clients_flux_rules",
                column: "peer_group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_clients_flux_rules_peer_groups_peer_group_id",
                table: "clients_flux_rules",
                column: "peer_group_id",
                principalTable: "peer_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_clients_flux_rules_peer_groups_peer_group_id",
                table: "clients_flux_rules");

            migrationBuilder.DropIndex(
                name: "ix_clients_flux_rules_peer_group_id",
                table: "clients_flux_rules");
        }
    }
}
