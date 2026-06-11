using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exercise_logs_workout_sessions_WorkoutSessionId1",
                table: "exercise_logs");

            migrationBuilder.DropIndex(
                name: "IX_exercise_logs_WorkoutSessionId1",
                table: "exercise_logs");

            migrationBuilder.DropColumn(
                name: "WorkoutSessionId1",
                table: "exercise_logs");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_logs_WorkoutSessionId",
                table: "exercise_logs",
                column: "WorkoutSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_exercise_logs_workout_sessions_WorkoutSessionId",
                table: "exercise_logs",
                column: "WorkoutSessionId",
                principalTable: "workout_sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exercise_logs_workout_sessions_WorkoutSessionId",
                table: "exercise_logs");

            migrationBuilder.DropIndex(
                name: "IX_exercise_logs_WorkoutSessionId",
                table: "exercise_logs");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkoutSessionId1",
                table: "exercise_logs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_exercise_logs_WorkoutSessionId1",
                table: "exercise_logs",
                column: "WorkoutSessionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_exercise_logs_workout_sessions_WorkoutSessionId1",
                table: "exercise_logs",
                column: "WorkoutSessionId1",
                principalTable: "workout_sessions",
                principalColumn: "Id");
        }
    }
}
