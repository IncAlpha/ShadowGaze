using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ShadowGaze.Data.Migrations
{
    /// <inheritdoc />
    public partial class PromotionalCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "promotional_codes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    value = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotional_codes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "promotional_code_usages",
                columns: table => new
                {
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    promotional_code_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotional_code_usages", x => new { x.customer_id, x.promotional_code_id });
                    table.ForeignKey(
                        name: "fk_promotional_code_usages_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_promotional_code_usages_promotional_codes_promotional_code_",
                        column: x => x.promotional_code_id,
                        principalTable: "promotional_codes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_promotional_code_usages_promotional_code_id",
                table: "promotional_code_usages",
                column: "promotional_code_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "promotional_code_usages");

            migrationBuilder.DropTable(
                name: "promotional_codes");
        }
    }
}
