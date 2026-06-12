using FitnessTracker.Domain.Entities;

namespace FitnessTracker.Application.Common.Interfaces;

public interface IExerciseRepository
{
    Task<Exercise?> FindByNameAsync(string name, CancellationToken ct = default);
    Task<Exercise?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Exercise>> SearchAsync(string query, CancellationToken ct = default);
    Task<IReadOnlyList<Exercise>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Exercise exercise, CancellationToken ct = default);
}
