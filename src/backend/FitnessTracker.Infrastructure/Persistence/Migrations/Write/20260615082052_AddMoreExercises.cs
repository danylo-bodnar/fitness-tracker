using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitnessTracker.Infrastructure.Persistence.Migrations.Write
{
    /// <inheritdoc />
    public partial class AddMoreExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000001"),
                column: "MuscleGroup",
                value: "Biceps");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000001"),
                column: "MuscleGroup",
                value: "Biceps");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000a-0000-0000-0000-000000000001"),
                column: "MuscleGroup",
                value: "Triceps");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000c-0000-0000-0000-000000000001"),
                column: "MuscleGroup",
                value: "Triceps");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000d-0000-0000-0000-000000000001"),
                column: "MuscleGroup",
                value: "Triceps");

            migrationBuilder.InsertData(
                table: "exercises",
                columns: new[] { "Id", "MuscleGroup", "exercise_name" },
                values: new object[,]
                {
                    { new Guid("00000001-0000-0000-0000-000000000002"), "Biceps", "incline dumbbell curl" },
                    { new Guid("00000002-0000-0000-0000-000000000002"), "Biceps", "cable curl" },
                    { new Guid("00000003-0000-0000-0000-000000000002"), "Triceps", "skull crusher" },
                    { new Guid("00000004-0000-0000-0000-000000000002"), "Triceps", "close grip bench press" },
                    { new Guid("00000005-0000-0000-0000-000000000002"), "Chest", "incline bench press" },
                    { new Guid("00000006-0000-0000-0000-000000000002"), "Chest", "cable fly" },
                    { new Guid("00000007-0000-0000-0000-000000000002"), "Chest", "pec deck" },
                    { new Guid("00000008-0000-0000-0000-000000000002"), "Shoulders", "overhead press" },
                    { new Guid("00000009-0000-0000-0000-000000000002"), "Shoulders", "front raises" },
                    { new Guid("0000000a-0000-0000-0000-000000000002"), "Shoulders", "face pull" },
                    { new Guid("0000000b-0000-0000-0000-000000000002"), "Shoulders", "arnold press" },
                    { new Guid("0000000c-0000-0000-0000-000000000002"), "Back", "lat pulldown" },
                    { new Guid("0000000d-0000-0000-0000-000000000002"), "Back", "deadlift" },
                    { new Guid("0000000e-0000-0000-0000-000000000002"), "Back", "t-bar row" },
                    { new Guid("0000000f-0000-0000-0000-000000000002"), "Legs", "leg extension" },
                    { new Guid("00000010-0000-0000-0000-000000000002"), "Legs", "lunges" },
                    { new Guid("00000011-0000-0000-0000-000000000002"), "Legs", "hip thrust" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000003-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000004-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000005-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000006-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000007-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000008-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000009-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000a-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000b-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000c-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000d-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000e-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000f-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000010-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000011-0000-0000-0000-000000000002"));

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000001"),
                column: "MuscleGroup",
                value: "Arms");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000001"),
                column: "MuscleGroup",
                value: "Arms");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000a-0000-0000-0000-000000000001"),
                column: "MuscleGroup",
                value: "Chest");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000c-0000-0000-0000-000000000001"),
                column: "MuscleGroup",
                value: "Arms");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("0000000d-0000-0000-0000-000000000001"),
                column: "MuscleGroup",
                value: "Arms");
        }
    }
}
