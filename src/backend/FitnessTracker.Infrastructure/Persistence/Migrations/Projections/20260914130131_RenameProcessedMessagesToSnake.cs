using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Persistence.Migrations.Projections
{
    /// <inheritdoc />
    public partial class RenameProcessedMessagesToSnake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessedAt",
                table: "processed_messages",
                newName: "processed_at");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "processed_messages",
                newName: "event_id");

            migrationBuilder.RenameColumn(
                name: "ConsumerName",
                table: "processed_messages",
                newName: "consumer_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "processed_at",
                table: "processed_messages",
                newName: "ProcessedAt");

            migrationBuilder.RenameColumn(
                name: "event_id",
                table: "processed_messages",
                newName: "EventId");

            migrationBuilder.RenameColumn(
                name: "consumer_name",
                table: "processed_messages",
                newName: "ConsumerName");
        }
    }
}
