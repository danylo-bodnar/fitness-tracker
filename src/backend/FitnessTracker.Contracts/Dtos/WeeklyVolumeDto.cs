namespace FitnessTracker.Contracts.Dtos;

public record WeeklyVolumeDto(
    Guid Id,
    DateOnly WeekStart,
    decimal TotalVolume,
    int SessionCount
);
