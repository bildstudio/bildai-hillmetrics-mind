using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HillMetrics.MIND.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class PeerGroup_SearchPresets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    s_id = table.Column<string>(type: "text", nullable: true),
                    first_name = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    dt_insert = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "peer_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    computation_frequency = table.Column<int>(type: "integer", nullable: false),
                    dt_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    dt_insert = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_peer_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_peer_groups_users_user_id",
                        column: x => x.user_id,
                        principalTable: "user_accounts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "search_preset",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    filters = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    dt_insert = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_search_preset", x => x.id);
                    table.ForeignKey(
                        name: "fk_search_preset_users_user_id",
                        column: x => x.user_id,
                        principalTable: "user_accounts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "peer_group_computation_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    peer_group_id = table.Column<int>(type: "integer", nullable: false),
                    computed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total_records = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    status_details = table.Column<string>(type: "text", nullable: true),
                    dt_insert = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_peer_group_computation_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_peer_group_computation_history_peer_groups_peer_group_id",
                        column: x => x.peer_group_id,
                        principalTable: "peer_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "peer_groups_engines",
                columns: table => new
                {
                    peer_group_id = table.Column<int>(type: "integer", nullable: false),
                    search_preset_id = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    @operator = table.Column<int>(name: "operator", type: "integer", nullable: false),
                    dt_insert = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_peer_groups_engines", x => new { x.peer_group_id, x.search_preset_id });
                    table.ForeignKey(
                        name: "fk_peer_groups_engines_peer_groups_peer_group_id",
                        column: x => x.peer_group_id,
                        principalTable: "peer_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_peer_groups_engines_search_presets_search_preset_id",
                        column: x => x.search_preset_id,
                        principalTable: "search_preset",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "peer_group_computation_history_results",
                columns: table => new
                {
                    financial_id = table.Column<int>(type: "integer", nullable: false),
                    history_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_peer_group_computation_history_results", x => new { x.financial_id, x.history_id });
                    table.ForeignKey(
                        name: "fk_peer_group_computation_history_results_peer_group_computati",
                        column: x => x.history_id,
                        principalTable: "peer_group_computation_history",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_peer_group_computation_history_peer_group_id",
                table: "peer_group_computation_history",
                column: "peer_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_peer_group_computation_history_results_history_id",
                table: "peer_group_computation_history_results",
                column: "history_id");

            migrationBuilder.CreateIndex(
                name: "ix_peer_groups_user_id",
                table: "peer_groups",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_peer_groups_engines_search_preset_id",
                table: "peer_groups_engines",
                column: "search_preset_id");

            migrationBuilder.CreateIndex(
                name: "ix_search_preset_user_id",
                table: "search_preset",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "peer_group_computation_history_results");

            migrationBuilder.DropTable(
                name: "peer_groups_engines");

            migrationBuilder.DropTable(
                name: "peer_group_computation_history");

            migrationBuilder.DropTable(
                name: "search_preset");

            migrationBuilder.DropTable(
                name: "peer_groups");

            migrationBuilder.DropTable(
                name: "user_accounts");
        }
    }
}
