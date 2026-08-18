using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public class WorkoutSessionRepository(AppDbContext db) : IWorkoutSessionRepository
{
    private readonly AppDbContext _db = db;

    public async Task<WorkoutSession?> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken ct = default)
    {
        return await _db.WorkoutSessions
            .Include(x => x.Exercises)
            .ThenInclude(x => x.Sets)
            .Where(x => x.UserId == userId && x.Date == date)
            .AsSplitQuery()
            .SingleOrDefaultAsync(ct);
    }

    public void Add(WorkoutSession session)
    {
        _db.WorkoutSessions.Add(session);
    }
}
