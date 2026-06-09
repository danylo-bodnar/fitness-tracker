using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Interfaces;

public interface IWorkoutSessionRepository
{
    Task<WorkoutSession?> GetByUserAndDateAsync(UserId userId, DateOnly date, CancellationToken ct = default);
    Task AddAsync(WorkoutSession session, CancellationToken ct = default);
}
