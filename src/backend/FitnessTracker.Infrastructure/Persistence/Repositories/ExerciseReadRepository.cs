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

    public async Task<List<ExerciseDto>> SearchAsync(string query, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                id            AS "Id",
                exercise_name AS "Name",
                muscle_group  AS "MuscleGroup"
            FROM exercises
            WHERE exercise_name ILIKE @query
            ORDER BY exercise_name
            LIMIT 10
            """;

        await using var connection = await dataSource.OpenConnectionAsync(ct);

        var result = await connection.QueryAsync<ExerciseDto>(
            new CommandDefinition(sql, new { query = $"%{query}%" }, cancellationToken: ct));

        return result.ToList();
    }

}