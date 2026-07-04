using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddOrderToProgramDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "program_days",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order",
                table: "program_days");
        }
    }
}
