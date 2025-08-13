using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShadowGaze.Data.Migrations
{
    /// <inheritdoc />
    public partial class TelegramId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "endpoint_id",
                table: "customers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "telegram_id",
                table: "customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "telegram_id",
                table: "customers");

            migrationBuilder.AlterColumn<int>(
                name: "endpoint_id",
                table: "customers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
