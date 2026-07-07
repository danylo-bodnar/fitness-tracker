using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitnessTracker.Infrastructure.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class InitAppDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    exercise_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    muscle_group = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercises", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "InboxState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LockId = table.Column<Guid>(type: "uuid", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReceiveCount = table.Column<int>(type: "integer", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Consumed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnqueueTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Headers = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutboxId = table.Column<Guid>(type: "uuid", nullable: true),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber);
                });

            migrationBuilder.CreateTable(
                name: "OutboxState",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "uuid", nullable: false),
                    LockId = table.Column<Guid>(type: "uuid", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    telegram_chat_id = table.Column<long>(type: "bigint", nullable: false),
                    telegram_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workout_programs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_programs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workout_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_sessions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "program_days",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    workout_program_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_program_days", x => x.id);
                    table.ForeignKey(
                        name: "FK_program_days_workout_programs_workout_program_id",
                        column: x => x.workout_program_id,
                        principalTable: "workout_programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exercise_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    exercise_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exercise_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    workout_session_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_exercise_logs_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exercise_logs_workout_sessions_workout_session_id",
                        column: x => x.workout_session_id,
                        principalTable: "workout_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "program_exercises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    exercise_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exercise_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    target_sets = table.Column<int>(type: "integer", nullable: false),
                    target_reps = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    program_day_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_program_exercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_program_exercises_program_days_program_day_id",
                        column: x => x.program_day_id,
                        principalTable: "program_days",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    repetitions = table.Column<int>(type: "integer", nullable: false),
                    exercise_log_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sets", x => x.id);
                    table.ForeignKey(
                        name: "FK_sets_exercise_logs_exercise_log_id",
                        column: x => x.exercise_log_id,
                        principalTable: "exercise_logs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "exercises",
                columns: new[] { "id", "muscle_group", "exercise_name" },
                values: new object[,]
                {
                    { new Guid("00000001-0000-0000-0000-000000000001"), "Biceps", "bicep curl" },
                    { new Guid("00000001-0000-0000-0000-000000000002"), "Biceps", "incline dumbbell curl" },
                    { new Guid("00000002-0000-0000-0000-000000000001"), "Biceps", "hammer curl" },
                    { new Guid("00000002-0000-0000-0000-000000000002"), "Biceps", "cable curl" },
                    { new Guid("00000003-0000-0000-0000-000000000001"), "Legs", "squat" },
                    { new Guid("00000003-0000-0000-0000-000000000002"), "Triceps", "skull crusher" },
                    { new Guid("00000004-0000-0000-0000-000000000001"), "Legs", "leg press" },
                    { new Guid("00000004-0000-0000-0000-000000000002"), "Triceps", "close grip bench press" },
                    { new Guid("00000005-0000-0000-0000-000000000001"), "Legs", "leg curl" },
                    { new Guid("00000005-0000-0000-0000-000000000002"), "Chest", "incline bench press" },
                    { new Guid("00000006-0000-0000-0000-000000000001"), "Legs", "calf raises" },
                    { new Guid("00000006-0000-0000-0000-000000000002"), "Chest", "cable fly" },
                    { new Guid("00000007-0000-0000-0000-000000000001"), "Legs", "romanian deadlift" },
                    { new Guid("00000007-0000-0000-0000-000000000002"), "Chest", "pec deck" },
                    { new Guid("00000008-0000-0000-0000-000000000001"), "Chest", "bench press" },
                    { new Guid("00000008-0000-0000-0000-000000000002"), "Shoulders", "overhead press" },
                    { new Guid("00000009-0000-0000-0000-000000000001"), "Chest", "incline dumbbell press" },
                    { new Guid("00000009-0000-0000-0000-000000000002"), "Shoulders", "front raises" },
                    { new Guid("0000000a-0000-0000-0000-000000000001"), "Triceps", "dips" },
                    { new Guid("0000000a-0000-0000-0000-000000000002"), "Shoulders", "face pull" },
                    { new Guid("0000000b-0000-0000-0000-000000000001"), "Shoulders", "lateral raises" },
                    { new Guid("0000000b-0000-0000-0000-000000000002"), "Shoulders", "arnold press" },
                    { new Guid("0000000c-0000-0000-0000-000000000001"), "Triceps", "triceps pushdown" },
                    { new Guid("0000000c-0000-0000-0000-000000000002"), "Back", "lat pulldown" },
                    { new Guid("0000000d-0000-0000-0000-000000000001"), "Triceps", "triceps extension" },
                    { new Guid("0000000d-0000-0000-0000-000000000002"), "Back", "deadlift" },
                    { new Guid("0000000e-0000-0000-0000-000000000001"), "Back", "pull-ups" },
                    { new Guid("0000000e-0000-0000-0000-000000000002"), "Back", "t-bar row" },
                    { new Guid("0000000f-0000-0000-0000-000000000001"), "Back", "barbell row" },
                    { new Guid("0000000f-0000-0000-0000-000000000002"), "Legs", "leg extension" },
                    { new Guid("00000010-0000-0000-0000-000000000001"), "Back", "cable row" },
                    { new Guid("00000010-0000-0000-0000-000000000002"), "Legs", "lunges" },
                    { new Guid("00000011-0000-0000-0000-000000000001"), "Back", "machine row" },
                    { new Guid("00000011-0000-0000-0000-000000000002"), "Legs", "hip thrust" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_exercise_logs_exercise_id",
                table: "exercise_logs",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_logs_workout_session_id",
                table: "exercise_logs",
                column: "workout_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessage",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessage",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxState",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_program_days_workout_program_id",
                table: "program_days",
                column: "workout_program_id");

            migrationBuilder.CreateIndex(
                name: "IX_program_exercises_program_day_id",
                table: "program_exercises",
                column: "program_day_id");

            migrationBuilder.CreateIndex(
                name: "IX_sets_exercise_log_id",
                table: "sets",
                column: "exercise_log_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InboxState");

            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropTable(
                name: "OutboxState");

            migrationBuilder.DropTable(
                name: "program_exercises");

            migrationBuilder.DropTable(
                name: "sets");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "program_days");

            migrationBuilder.DropTable(
                name: "exercise_logs");

            migrationBuilder.DropTable(
                name: "workout_programs");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "workout_sessions");
        }
    }
}
