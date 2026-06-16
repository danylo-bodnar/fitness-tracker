using FitnessTracker.Domain.Aggregates;

namespace FitnessTracker.Application.Common.Interfaces;

public interface IWorkoutProgramRepository
{
    Task<WorkoutProgram?> GetByIdAsync(Guid id, CancellationToken ct = default);
    void Add(WorkoutProgram program);
    void Delete(WorkoutProgram program);
}
