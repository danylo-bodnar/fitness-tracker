namespace FitnessTracker.Infrastructure.RateLimiting;

public sealed class RateLimitResult
{
    public bool IsAllowed { get; init; }

    public int Remaining { get; init; }

    public int CurrentCount { get; init; }
}
