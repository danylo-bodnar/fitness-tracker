using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public class WorkoutSessionReadRepository(AppDbContext db) : IWorkoutSessionReadRepository
{
    private readonly AppDbContext _db = db;

    public async Task<PagedResultDto<WorkoutSessionDto>> GetHistoryAsync(
        Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        var totalCount = await _db.WorkoutSessions
            .AsNoTracking()
            .CountAsync(s => s.UserId == userId, ct);

        var items = await _db.WorkoutSessions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new WorkoutSessionDto(
                s.Id,
                s.Date,
                s.Exercises.Select(e => new WorkoutExerciseDto(
                    e.ExerciseId,
                    e.ExerciseName.Value,
                    e.Sets.Select(set => new WorkoutSetDto(
                        set.Weight.Kg,
                        set.Repetitions.Value
                    )).ToList()
                )).ToList()
            ))
            .ToListAsync(ct);

        return new PagedResultDto<WorkoutSessionDto>(items, totalCount, page, pageSize);
    }
}