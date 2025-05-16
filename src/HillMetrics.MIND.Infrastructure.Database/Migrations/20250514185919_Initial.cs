using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HillMetrics.MIND.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    dt_insert = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clients_flux_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    peer_group_id = table.Column<int>(type: "integer", nullable: false),
                    financial_data_point_id = table.Column<int>(type: "integer", nullable: false),
                    ranking = table.Column<int>(type: "integer", nullable: false),
                    dt_insert = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clients_flux_rules", x => x.id);
                    table.ForeignKey(
                        name: "fk_clients_flux_rules_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "clients_flux_priorities",
                columns: table => new
                {
                    client_flux_rule_entity_id = table.Column<int>(type: "integer", nullable: false),
                    flux_id = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clients_flux_priorities", x => new { x.client_flux_rule_entity_id, x.flux_id });
                    table.ForeignKey(
                        name: "fk_clients_flux_priorities_clients_flux_rules_client_flux_rule",
                        column: x => x.client_flux_rule_entity_id,
                        principalTable: "clients_flux_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_clients_flux_rules_client_id",
                table: "clients_flux_rules",
                column: "client_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clients_flux_priorities");

            migrationBuilder.DropTable(
                name: "clients_flux_rules");

            migrationBuilder.DropTable(
                name: "clients");
        }
    }
}
