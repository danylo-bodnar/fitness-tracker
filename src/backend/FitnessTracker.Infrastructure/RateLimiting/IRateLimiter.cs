namespace FitnessTracker.Infrastructure.RateLimiting;

public interface IRateLimiter
{
    Task<RateLimitResult> AllowAsync(string key);
}
