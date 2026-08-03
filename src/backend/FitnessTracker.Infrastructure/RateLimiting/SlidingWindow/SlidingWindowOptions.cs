namespace FitnessTracker.Infrastructure.RateLimiting.SlidingWindow;

public sealed class SlidingWindowOptions
{
    public const string SectionName = "RateLimiting:Authentication";

    public int MaxRequests { get; init; } = 5;

    public int WindowSeconds { get; init; } = 60;
}
