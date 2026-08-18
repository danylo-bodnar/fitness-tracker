using FitnessTracker.Contracts.Dtos;

namespace FitnessTracker.Application.Common.Interfaces;

public interface IWorkoutSessionReadRepository
{
    Task<PagedResultDto<WorkoutSessionDto>> GetHistoryAsync(
        Guid userId, int page, int pageSize, CancellationToken ct = default);
}