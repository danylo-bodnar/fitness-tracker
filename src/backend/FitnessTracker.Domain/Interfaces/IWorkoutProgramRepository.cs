using FitnessTracker.Domain.Aggregates;

namespace FitnessTracker.Domain.Interfaces;

public interface IWorkoutProgramRepository
{
    Task<WorkoutProgram?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<WorkoutProgram>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    void Add(WorkoutProgram program);
    void Update(WorkoutProgram program);
    void Delete(WorkoutProgram program);
}