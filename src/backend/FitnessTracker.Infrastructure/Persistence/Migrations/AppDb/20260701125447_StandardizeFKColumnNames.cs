using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class StandardizeFKColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_program_exercises_program_days_ProgramDayId",
                table: "program_exercises");

            migrationBuilder.RenameColumn(
                name: "ProgramDayId",
                table: "program_exercises",
                newName: "program_day_id");

            migrationBuilder.RenameIndex(
                name: "IX_program_exercises_ProgramDayId",
                table: "program_exercises",
                newName: "IX_program_exercises_program_day_id");

            migrationBuilder.AddForeignKey(
                name: "FK_program_exercises_program_days_program_day_id",
                table: "program_exercises",
                column: "program_day_id",
                principalTable: "program_days",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_program_exercises_program_days_program_day_id",
                table: "program_exercises");

            migrationBuilder.RenameColumn(
                name: "program_day_id",
                table: "program_exercises",
                newName: "ProgramDayId");

            migrationBuilder.RenameIndex(
                name: "IX_program_exercises_program_day_id",
                table: "program_exercises",
                newName: "IX_program_exercises_ProgramDayId");

            migrationBuilder.AddForeignKey(
                name: "FK_program_exercises_program_days_ProgramDayId",
                table: "program_exercises",
                column: "ProgramDayId",
                principalTable: "program_days",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
