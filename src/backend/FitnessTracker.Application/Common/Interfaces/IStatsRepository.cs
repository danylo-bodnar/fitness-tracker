using FitnessTracker.Contracts.Dtos;

namespace FitnessTracker.Application.Common.Interfaces;

public interface IStatsRepository
{
    Task<DashboardStatsDto?> GetDashboardAsync(Guid userId, CancellationToken ct = default);
    Task<List<PersonalRecordDto>> GetPersonalRecordsAsync(Guid userId, Guid? exerciseId = null, CancellationToken ct = default);
    Task<List<ExerciseProgressDto>> GetExerciseProgressAsync(Guid userId, Guid exerciseId, CancellationToken ct = default);
    Task<List<WeeklyVolumeDto>> GetWeeklyVolumeAsync(Guid userId, int weeks = 12, CancellationToken ct = default);
}
