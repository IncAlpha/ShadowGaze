using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ShadowGaze.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTelegramFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_id",
                table: "platform_instructions");

            migrationBuilder.AddColumn<int>(
                name: "telegram_file_id",
                table: "platform_instructions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "telegram_files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_id = table.Column<string>(type: "text", nullable: true),
                    file_unique_id = table.Column<string>(type: "text", nullable: true),
                    file_type = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_files", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bot_sections",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    section_name = table.Column<string>(type: "text", nullable: true),
                    telegram_file_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bot_sections", x => x.id);
                    table.ForeignKey(
                        name: "fk_bot_sections_telegram_files_telegram_file_id",
                        column: x => x.telegram_file_id,
                        principalTable: "telegram_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_platform_instructions_telegram_file_id",
                table: "platform_instructions",
                column: "telegram_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_bot_sections_telegram_file_id",
                table: "bot_sections",
                column: "telegram_file_id");

            migrationBuilder.AddForeignKey(
                name: "fk_platform_instructions_telegram_files_telegram_file_id",
                table: "platform_instructions",
                column: "telegram_file_id",
                principalTable: "telegram_files",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_platform_instructions_telegram_files_telegram_file_id",
                table: "platform_instructions");

            migrationBuilder.DropTable(
                name: "bot_sections");

            migrationBuilder.DropTable(
                name: "telegram_files");

            migrationBuilder.DropIndex(
                name: "ix_platform_instructions_telegram_file_id",
                table: "platform_instructions");

            migrationBuilder.DropColumn(
                name: "telegram_file_id",
                table: "platform_instructions");

            migrationBuilder.AddColumn<string>(
                name: "file_id",
                table: "platform_instructions",
                type: "text",
                nullable: true);
        }
    }
}
