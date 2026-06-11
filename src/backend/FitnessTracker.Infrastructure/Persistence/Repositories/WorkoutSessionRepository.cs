using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Interfaces;
using FitnessTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public class WorkoutSessionRepository(WriteDbContext db) : IWorkoutSessionRepository
{
    private readonly WriteDbContext _db = db;

    public async Task<WorkoutSession?> GetByUserAndDateAsync(UserId userId, DateOnly date, CancellationToken ct = default)
    {
        return await _db.WorkoutSessions
            .Include(x => x.Exercises)
            .ThenInclude(x => x.Sets)
            .Where(x => x.UserId == userId && x.Date == date)
            .SingleOrDefaultAsync(ct);
    }

    public void Add(WorkoutSession session)
    {
        _db.WorkoutSessions.Add(session);
    }
}
