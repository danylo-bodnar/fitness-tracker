using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Persistence.Migrations.Projections
{
    /// <inheritdoc />
    public partial class ReworkProcessedMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_processed_messages",
                table: "processed_messages");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "processed_messages",
                newName: "EventId");

            migrationBuilder.AddColumn<string>(
                name: "ConsumerName",
                table: "processed_messages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_processed_messages",
                table: "processed_messages",
                columns: new[] { "ConsumerName", "EventId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_processed_messages",
                table: "processed_messages");

            migrationBuilder.DropColumn(
                name: "ConsumerName",
                table: "processed_messages");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "processed_messages",
                newName: "MessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_processed_messages",
                table: "processed_messages",
                column: "MessageId");
        }
    }
}
