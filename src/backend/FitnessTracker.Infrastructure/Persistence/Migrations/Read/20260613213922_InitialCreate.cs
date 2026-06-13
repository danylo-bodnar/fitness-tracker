using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Persistence.Migrations.Read
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dashboard_stats",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalSessions = table.Column<int>(type: "integer", nullable: false),
                    TotalVolumeKg = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    LastWorkoutAt = table.Column<DateOnly>(type: "date", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dashboard_stats", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "exercise_progress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WorkoutDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MaxWeightKg = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    TotalVolume = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    SetCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_progress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_prs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WeightKg = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false),
                    Estimated1RM = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    AchievedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_prs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "weekly_volume",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekStart = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalVolume = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    SessionCount = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weekly_volume", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_exercise_progress_UserId_ExerciseId",
                table: "exercise_progress",
                columns: new[] { "UserId", "ExerciseId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_prs_UserId_ExerciseId",
                table: "user_prs",
                columns: new[] { "UserId", "ExerciseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_weekly_volume_UserId_WeekStart",
                table: "weekly_volume",
                columns: new[] { "UserId", "WeekStart" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dashboard_stats");

            migrationBuilder.DropTable(
                name: "exercise_progress");

            migrationBuilder.DropTable(
                name: "user_prs");

            migrationBuilder.DropTable(
                name: "weekly_volume");
        }
    }
}
