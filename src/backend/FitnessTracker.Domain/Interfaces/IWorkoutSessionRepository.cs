using FitnessTracker.Domain.Aggregates;

namespace FitnessTracker.Domain.Interfaces;

public interface IWorkoutSessionRepository
{
    Task<WorkoutSession?> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken ct = default);
    void Add(WorkoutSession session);
}
