using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddSupersetGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "superset_group_id",
                table: "program_exercises",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "superset_group_id",
                table: "exercise_logs",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "superset_group_id",
                table: "program_exercises");

            migrationBuilder.DropColumn(
                name: "superset_group_id",
                table: "exercise_logs");
        }
    }
}
