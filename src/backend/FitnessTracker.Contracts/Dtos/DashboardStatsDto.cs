namespace FitnessTracker.Contracts.Dtos;

public record DashboardStatsDto(
    int TotalSessions,
    decimal TotalVolumeKg,
    DateOnly? LastWorkoutAt
);
