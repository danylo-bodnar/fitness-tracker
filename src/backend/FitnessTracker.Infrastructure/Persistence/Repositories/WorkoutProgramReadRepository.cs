using Dapper;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using Npgsql;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public class WorkoutProgramReadRepository(NpgsqlDataSource dataSource) : IWorkoutProgramReadRepository
{
    public async Task<List<WorkoutProgramDto>> GetByUserAsync(Guid userId, CancellationToken ct = default)
    {
        const string sql = """
        SELECT
            p.id         AS "Id",
            p.name       AS "Name",
            d.id         AS "DayId",
            d.name       AS "DayName",
            e.exercise_id   AS "ExerciseId",
            e.exercise_name AS "ExerciseName",
            e.target_sets   AS "TargetSets",
            e.target_reps   AS "TargetReps",
            e.order         AS "Order"
        FROM workout_programs p
        LEFT JOIN program_days      d ON d.workout_program_id = p.id
        LEFT JOIN program_exercises e ON e.program_day_id     = d.id
        WHERE p.user_id = @userId
        ORDER BY p.id, d.id, e.order
        """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var programLookup = new Dictionary<Guid, WorkoutProgramDto>();
        var dayLookup = new Dictionary<Guid, List<ProgramExerciseDto>>();

        await connection.QueryAsync<ProgramRow, DayRow, ExerciseRow, WorkoutProgramDto>(
            new CommandDefinition(sql, new { userId }, cancellationToken: ct),
            (program, day, exercise) =>
            {
                if (!programLookup.TryGetValue(program.Id, out var programDto))
                {
                    programDto = new WorkoutProgramDto
                    {
                        Id = program.Id,
                        Name = program.Name,
                        Days = []
                    };
                    programLookup[program.Id] = programDto;
                }

                if (day is null)
                    return programDto;

                if (!dayLookup.TryGetValue(day.DayId, out var exercises))
                {
                    exercises = [];
                    var dayDto = new ProgramDayDto(day.DayId, day.DayName, exercises);
                    dayLookup[day.DayId] = exercises;
                    programDto.Days.Add(dayDto);
                }

                if (exercise is not null)
                {
                    exercises.Add(new ProgramExerciseDto(
                        exercise.ExerciseId,
                        exercise.ExerciseName,
                        exercise.TargetSets,
                        exercise.TargetReps,
                        exercise.Order));
                }

                return programDto;
            },
            splitOn: "DayId,ExerciseId"
        );

        return [.. programLookup.Values];
    }

    private record ProgramRow(Guid Id, string Name);
    private record DayRow(Guid DayId, string DayName);
    private record ExerciseRow(Guid ExerciseId, string ExerciseName, int TargetSets, int TargetReps, int Order);
}
