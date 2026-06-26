using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public class StatsRepository(ProjectionsDbContext db) : IStatsRepository
{
    public async Task<DashboardStatsDto?> GetDashboardAsync(Guid userId, CancellationToken ct = default)
    {
        var stats = await db.DashboardStats
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Select(s => new DashboardStatsDto(
                s.TotalSessions,
                s.TotalVolumeKg,
                s.LastWorkoutAt))
            .SingleOrDefaultAsync(ct);

        return stats ?? new DashboardStatsDto(0, 0, null);
    }

    public async Task<List<PersonalRecordDto>> GetPersonalRecordsAsync(
        Guid userId, Guid? exerciseId = null, CancellationToken ct = default)
    {
        var query = db.UserPRs
            .AsNoTracking()
            .Where(r => r.UserId == userId);

        if (exerciseId.HasValue)
            query = query.Where(r => r.ExerciseId == exerciseId.Value);

        return await query
            .OrderByDescending(r => r.AchievedAt)
            .Select(r => new PersonalRecordDto(
                r.Id,
                r.ExerciseId,
                r.ExerciseName,
                r.WeightKg,
                r.Reps,
                r.Estimated1RM,
                r.AchievedAt))
            .ToListAsync(ct);
    }

    public async Task<List<ExerciseProgressDto>> GetExerciseProgressAsync(
        Guid userId, Guid exerciseId, CancellationToken ct = default)
    {
        return await db.ExerciseProgress
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.ExerciseId == exerciseId)
            .OrderBy(p => p.WorkoutDate)
            .Select(p => new ExerciseProgressDto(
                p.Id,
                p.ExerciseId,
                p.ExerciseName,
                p.WorkoutDate,
                p.MaxWeightKg,
                p.TotalVolume,
                p.SetCount))
            .ToListAsync(ct);
    }

    public async Task<List<WeeklyVolumeDto>> GetWeeklyVolumeAsync(
        Guid userId, int weeks = 12, CancellationToken ct = default)
    {
        var since = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7 * weeks));

        return await db.WeeklyVolume
            .AsNoTracking()
            .Where(v => v.UserId == userId && v.WeekStart >= since)
            .OrderBy(v => v.WeekStart)
            .Select(v => new WeeklyVolumeDto(
                v.Id,
                v.WeekStart,
                v.TotalVolume,
                v.SessionCount))
            .ToListAsync(ct);
    }
}
