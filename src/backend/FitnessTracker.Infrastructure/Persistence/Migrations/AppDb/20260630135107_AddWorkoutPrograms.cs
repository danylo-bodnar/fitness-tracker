using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddWorkoutPrograms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_program_days_WorkoutPrograms_WorkoutProgramId",
                table: "program_days");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutPrograms",
                table: "WorkoutPrograms");

            migrationBuilder.RenameTable(
                name: "WorkoutPrograms",
                newName: "workout_programs");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "workout_programs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_workout_programs",
                table: "workout_programs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_program_days_workout_programs_WorkoutProgramId",
                table: "program_days",
                column: "WorkoutProgramId",
                principalTable: "workout_programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_program_days_workout_programs_WorkoutProgramId",
                table: "program_days");

            migrationBuilder.DropPrimaryKey(
                name: "PK_workout_programs",
                table: "workout_programs");

            migrationBuilder.RenameTable(
                name: "workout_programs",
                newName: "WorkoutPrograms");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WorkoutPrograms",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutPrograms",
                table: "WorkoutPrograms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_program_days_WorkoutPrograms_WorkoutProgramId",
                table: "program_days",
                column: "WorkoutProgramId",
                principalTable: "WorkoutPrograms",
                principalColumn: "Id");
        }
    }
}
