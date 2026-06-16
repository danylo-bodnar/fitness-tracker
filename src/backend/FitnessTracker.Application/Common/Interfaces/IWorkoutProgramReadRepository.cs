using FitnessTracker.Contracts.Dtos;

namespace FitnessTracker.Application.Common.Interfaces;

public interface IWorkoutProgramReadRepository
{
    Task<List<WorkoutProgramDto>> GetByUserAsync(Guid userId);
}
