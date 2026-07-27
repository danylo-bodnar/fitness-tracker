using FitnessTracker.Contracts.Dtos;

namespace FitnessTracker.Application.Common.Interfaces;

public interface IWorkoutProgramReadRepository
{
    Task<List<WorkoutProgramDto>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<WorkoutProgramDto?> GetByIdAsync(Guid programId, Guid userId, CancellationToken ct = default);
}