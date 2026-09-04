using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Infrastructure.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddInclineLegPress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "exercises",
                columns: new[] { "id", "muscle_group", "exercise_name" },
                values: new object[] { new Guid("00000012-0000-0000-0000-000000000001"), "Legs", "incline leg press" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("00000012-0000-0000-0000-000000000001"));
        }
    }
}
