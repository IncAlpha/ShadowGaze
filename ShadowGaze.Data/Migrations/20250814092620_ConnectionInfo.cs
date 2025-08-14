using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShadowGaze.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConnectionInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "connection_string",
                table: "endpoints",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "qr_code",
                table: "endpoints",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "connection_string",
                table: "endpoints");

            migrationBuilder.DropColumn(
                name: "qr_code",
                table: "endpoints");
        }
    }
}
