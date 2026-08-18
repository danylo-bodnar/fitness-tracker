using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class EnsureUniqueTelegramChatId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_users_telegram_chat_id",
                table: "users",
                column: "telegram_chat_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_telegram_chat_id",
                table: "users");
        }
    }
}
