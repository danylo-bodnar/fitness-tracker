using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class StandardizeColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exercise_logs_exercises_ExerciseId",
                table: "exercise_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_exercise_logs_workout_sessions_WorkoutSessionId",
                table: "exercise_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_program_days_workout_programs_WorkoutProgramId",
                table: "program_days");

            migrationBuilder.DropForeignKey(
                name: "FK_sets_exercise_logs_ExerciseLogId",
                table: "sets");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "workout_sessions",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "workout_sessions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "workout_sessions",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "workout_programs",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "workout_programs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "workout_programs",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "sets",
                newName: "weight");

            migrationBuilder.RenameColumn(
                name: "Repetitions",
                table: "sets",
                newName: "repetitions");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "sets",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ExerciseLogId",
                table: "sets",
                newName: "exercise_log_id");

            migrationBuilder.RenameIndex(
                name: "IX_sets_ExerciseLogId",
                table: "sets",
                newName: "IX_sets_exercise_log_id");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "program_exercises",
                newName: "order");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "program_exercises",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TargetSets",
                table: "program_exercises",
                newName: "target_sets");

            migrationBuilder.RenameColumn(
                name: "TargetReps",
                table: "program_exercises",
                newName: "target_reps");

            migrationBuilder.RenameColumn(
                name: "ExerciseName",
                table: "program_exercises",
                newName: "exercise_name");

            migrationBuilder.RenameColumn(
                name: "ExerciseId",
                table: "program_exercises",
                newName: "exercise_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "program_days",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "program_days",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WorkoutProgramId",
                table: "program_days",
                newName: "workout_program_id");

            migrationBuilder.RenameIndex(
                name: "IX_program_days_WorkoutProgramId",
                table: "program_days",
                newName: "IX_program_days_workout_program_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "exercises",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "MuscleGroup",
                table: "exercises",
                newName: "muscle_group");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "exercise_logs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WorkoutSessionId",
                table: "exercise_logs",
                newName: "workout_session_id");

            migrationBuilder.RenameColumn(
                name: "ExerciseId",
                table: "exercise_logs",
                newName: "exercise_id");

            migrationBuilder.RenameIndex(
                name: "IX_exercise_logs_WorkoutSessionId",
                table: "exercise_logs",
                newName: "IX_exercise_logs_workout_session_id");

            migrationBuilder.RenameIndex(
                name: "IX_exercise_logs_ExerciseId",
                table: "exercise_logs",
                newName: "IX_exercise_logs_exercise_id");

            migrationBuilder.AddForeignKey(
                name: "FK_exercise_logs_exercises_exercise_id",
                table: "exercise_logs",
                column: "exercise_id",
                principalTable: "exercises",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exercise_logs_workout_sessions_workout_session_id",
                table: "exercise_logs",
                column: "workout_session_id",
                principalTable: "workout_sessions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_program_days_workout_programs_workout_program_id",
                table: "program_days",
                column: "workout_program_id",
                principalTable: "workout_programs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sets_exercise_logs_exercise_log_id",
                table: "sets",
                column: "exercise_log_id",
                principalTable: "exercise_logs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exercise_logs_exercises_exercise_id",
                table: "exercise_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_exercise_logs_workout_sessions_workout_session_id",
                table: "exercise_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_program_days_workout_programs_workout_program_id",
                table: "program_days");

            migrationBuilder.DropForeignKey(
                name: "FK_sets_exercise_logs_exercise_log_id",
                table: "sets");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "workout_sessions",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "workout_sessions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "workout_sessions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "workout_programs",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "workout_programs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "workout_programs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "weight",
                table: "sets",
                newName: "Weight");

            migrationBuilder.RenameColumn(
                name: "repetitions",
                table: "sets",
                newName: "Repetitions");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "sets",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "exercise_log_id",
                table: "sets",
                newName: "ExerciseLogId");

            migrationBuilder.RenameIndex(
                name: "IX_sets_exercise_log_id",
                table: "sets",
                newName: "IX_sets_ExerciseLogId");

            migrationBuilder.RenameColumn(
                name: "order",
                table: "program_exercises",
                newName: "Order");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "program_exercises",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "target_sets",
                table: "program_exercises",
                newName: "TargetSets");

            migrationBuilder.RenameColumn(
                name: "target_reps",
                table: "program_exercises",
                newName: "TargetReps");

            migrationBuilder.RenameColumn(
                name: "exercise_name",
                table: "program_exercises",
                newName: "ExerciseName");

            migrationBuilder.RenameColumn(
                name: "exercise_id",
                table: "program_exercises",
                newName: "ExerciseId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "program_days",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "program_days",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "workout_program_id",
                table: "program_days",
                newName: "WorkoutProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_program_days_workout_program_id",
                table: "program_days",
                newName: "IX_program_days_WorkoutProgramId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "exercises",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "muscle_group",
                table: "exercises",
                newName: "MuscleGroup");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "exercise_logs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "workout_session_id",
                table: "exercise_logs",
                newName: "WorkoutSessionId");

            migrationBuilder.RenameColumn(
                name: "exercise_id",
                table: "exercise_logs",
                newName: "ExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_exercise_logs_workout_session_id",
                table: "exercise_logs",
                newName: "IX_exercise_logs_WorkoutSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_exercise_logs_exercise_id",
                table: "exercise_logs",
                newName: "IX_exercise_logs_ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_exercise_logs_exercises_ExerciseId",
                table: "exercise_logs",
                column: "ExerciseId",
                principalTable: "exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exercise_logs_workout_sessions_WorkoutSessionId",
                table: "exercise_logs",
                column: "WorkoutSessionId",
                principalTable: "workout_sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_program_days_workout_programs_WorkoutProgramId",
                table: "program_days",
                column: "WorkoutProgramId",
                principalTable: "workout_programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sets_exercise_logs_ExerciseLogId",
                table: "sets",
                column: "ExerciseLogId",
                principalTable: "exercise_logs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
