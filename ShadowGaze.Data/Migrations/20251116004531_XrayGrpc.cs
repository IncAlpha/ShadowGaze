using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ShadowGaze.Data.Migrations
{
    /// <inheritdoc />
    public partial class XrayGrpc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_customers_endpoints_endpoint_id",
                table: "customers");

            migrationBuilder.DropTable(
                name: "endpoints");

            migrationBuilder.DropTable(
                name: "xrays");

            migrationBuilder.DropIndex(
                name: "ix_customers_endpoint_id",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "balance",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "endpoint_id",
                table: "customers");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "customers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "expiry_date",
                table: "customers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "inbounds",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    obsolete = table.Column<bool>(type: "boolean", nullable: false),
                    api_uri = table.Column<string>(type: "text", nullable: true),
                    connection_uri = table.Column<string>(type: "text", nullable: true),
                    inbound_tag = table.Column<string>(type: "text", nullable: true),
                    protocol = table.Column<string>(type: "text", nullable: true),
                    flow = table.Column<string>(type: "text", nullable: true),
                    network = table.Column<string>(type: "text", nullable: true),
                    security = table.Column<string>(type: "text", nullable: true),
                    server_name = table.Column<string>(type: "text", nullable: true),
                    public_key = table.Column<string>(type: "text", nullable: true),
                    short_id = table.Column<string>(type: "text", nullable: true),
                    connection_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbounds", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "connection_buttons",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    button_name = table.Column<string>(type: "text", nullable: true),
                    inbound_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_connection_buttons", x => x.id);
                    table.ForeignKey(
                        name: "fk_connection_buttons_inbounds_inbound_id",
                        column: x => x.inbound_id,
                        principalTable: "inbounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "connections",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    vless_inbound_id = table.Column<int>(type: "integer", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    connection_string = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_connections", x => x.id);
                    table.ForeignKey(
                        name: "fk_connections_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_connections_inbounds_vless_inbound_id",
                        column: x => x.vless_inbound_id,
                        principalTable: "inbounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_connection_buttons_inbound_id",
                table: "connection_buttons",
                column: "inbound_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_connections_customer_id",
                table: "connections",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_connections_vless_inbound_id",
                table: "connections",
                column: "vless_inbound_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "connection_buttons");

            migrationBuilder.DropTable(
                name: "connections");

            migrationBuilder.DropTable(
                name: "inbounds");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "expiry_date",
                table: "customers");

            migrationBuilder.AddColumn<double>(
                name: "balance",
                table: "customers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "endpoint_id",
                table: "customers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "xrays",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    host = table.Column<string>(type: "text", nullable: true),
                    password = table.Column<string>(type: "text", nullable: true),
                    path = table.Column<string>(type: "text", nullable: true),
                    port = table.Column<int>(type: "integer", nullable: false),
                    username = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_xrays", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "endpoints",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    xray_id = table.Column<int>(type: "integer", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    connection_string = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    inbound_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_endpoints", x => x.id);
                    table.ForeignKey(
                        name: "fk_endpoints_xrays_xray_id",
                        column: x => x.xray_id,
                        principalTable: "xrays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_customers_endpoint_id",
                table: "customers",
                column: "endpoint_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_endpoints_xray_id",
                table: "endpoints",
                column: "xray_id");

            migrationBuilder.AddForeignKey(
                name: "fk_customers_endpoints_endpoint_id",
                table: "customers",
                column: "endpoint_id",
                principalTable: "endpoints",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
