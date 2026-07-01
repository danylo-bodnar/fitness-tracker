using FitnessTracker.Contracts.Dtos;

namespace FitnessTracker.Application.Common.Interfaces
{
    public interface IExerciseReadRepository
    {
        Task<List<ExerciseDto>> GetAllDefaultAsync(CancellationToken ct = default);
        Task<List<ExerciseDto>> SearchAsync(string query, CancellationToken ct = default);
    }
}