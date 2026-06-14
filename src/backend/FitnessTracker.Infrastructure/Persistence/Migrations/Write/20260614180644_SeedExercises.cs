using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitnessTracker.Infrastructure.Persistence.Migrations.Write
{
    /// <inheritdoc />
    public partial class SeedExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "exercises",
                columns: new[] { "Id", "MuscleGroup", "exercise_name" },
                values: new object[,]
                {
                    { new Guid("00000001-0000-0000-0000-000000000001"), "Arms", "bicep curl" },
                    { new Guid("00000002-0000-0000-0000-000000000001"), "Arms", "hammer curl" },
                    { new Guid("00000003-0000-0000-0000-000000000001"), "Legs", "squat" },
                    { new Guid("00000004-0000-0000-0000-000000000001"), "Legs", "leg press" },
                    { new Guid("00000005-0000-0000-0000-000000000001"), "Legs", "leg curl" },
                    { new Guid("00000006-0000-0000-0000-000000000001"), "Legs", "calf raises" },
                    { new Guid("00000007-0000-0000-0000-000000000001"), "Legs", "romanian deadlift" },
                    { new Guid("00000008-0000-0000-0000-000000000001"), "Chest", "bench press" },
                    { new Guid("00000009-0000-0000-0000-000000000001"), "Chest", "incline dumbbell press" },
                    { new Guid("0000000a-0000-0000-0000-000000000001"), "Chest", "dips" },
                    { new Guid("0000000b-0000-0000-0000-000000000001"), "Shoulders", "lateral raises" },
                    { new Guid("0000000c-0000-0000-0000-000000000001"), "Arms", "triceps pushdown" },
                    { new Guid("0000000d-0000-0000-0000-000000000001"), "Arms", "triceps extension" },
                    { new Guid("0000000e-0000-0000-0000-000000000001"), "Back", "pull-ups" },
                    { new Guid("0000000f-0000-0000-0000-000000000001"), "Back", "barbell row" },
                    { new Guid("00000010-0000-0000-0000-000000000001"), "Back", "cable row" },
                    { new Guid("00000011-0000-0000-0000-000000000001"), "Back", "machine row" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000003-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000004-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000005-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000006-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000007-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000008-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000009-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000a-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000b-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000c-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000d-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000e-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000f-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000010-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000011-0000-0000-0000-000000000001"));
        }
    }
}
