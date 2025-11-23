using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShadowGaze.Data.Migrations
{
    /// <inheritdoc />
    public partial class FieldRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_connections_connection_configurations_vless_inbound_id",
                table: "connections");

            migrationBuilder.DropIndex(
                name: "ix_connections_vless_inbound_id",
                table: "connections");

            migrationBuilder.RenameColumn(
                name: "vless_inbound_id",
                table: "connections",
                newName: "connection_configuration_id");

            migrationBuilder.CreateIndex(
                name: "ix_connections_connection_configuration_id",
                table: "connections",
                column: "connection_configuration_id");

            migrationBuilder.AddForeignKey(
                name: "fk_connections_connection_configurations_connection_configurat",
                table: "connections",
                column: "connection_configuration_id",
                principalTable: "connection_configurations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_connections_connection_configurations_connection_configurat",
                table: "connections");

            migrationBuilder.DropIndex(
                name: "ix_connections_connection_configuration_id",
                table: "connections");

            migrationBuilder.RenameColumn(
                name: "connection_configuration_id",
                table: "connections",
                newName: "vless_inbound_id");

            migrationBuilder.CreateIndex(
                name: "ix_connections_vless_inbound_id",
                table: "connections",
                column: "vless_inbound_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_connections_connection_configurations_vless_inbound_id",
                table: "connections",
                column: "vless_inbound_id",
                principalTable: "connection_configurations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
