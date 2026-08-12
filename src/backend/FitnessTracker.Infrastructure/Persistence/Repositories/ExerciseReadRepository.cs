using Dapper;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using Npgsql;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public class ExerciseReadRepository(NpgsqlDataSource dataSource) : IExerciseReadRepository
{
    public async Task<List<ExerciseDto>> GetAllDefaultAsync(CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                id           AS "Id",
                exercise_name AS "Name",
                muscle_group  AS "MuscleGroup"
            FROM exercises
            ORDER BY muscle_group, exercise_name
            """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var result = await connection.QueryAsync<ExerciseDto>(
            new CommandDefinition(sql, cancellationToken: ct));

        return result.ToList();
    }

    public async Task<List<ExerciseDto>> SearchAsync(string muscleGroup, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                id            AS "Id",
                exercise_name AS "Name",
                muscle_group  AS "MuscleGroup"
            FROM exercises
            WHERE muscle_group ILIKE @muscleGroup
            ORDER BY muscle_group, exercise_name
            """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var result = await connection.QueryAsync<ExerciseDto>(
            new CommandDefinition(sql, new { muscleGroup = $"%{muscleGroup}%" }, cancellationToken: ct));

        return result.ToList();
    }

}