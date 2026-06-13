namespace FitnessTracker.Infrastructure.Persistence.ReadModels;

public class WeeklyVolumeReadModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly WeekStart { get; set; }
    public decimal TotalVolume { get; set; }
    public int SessionCount { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class DashboardStatsReadModel
{
    public Guid UserId { get; set; }
    public int TotalSessions { get; set; }
    public decimal TotalVolumeKg { get; set; }
    public DateOnly? LastWorkoutAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
