namespace FitnessTracker.Api.RateLimiting;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class RateLimitAttribute : Attribute
{
    public RateLimitPolicy Policy { get; }

    public RateLimitAttribute(RateLimitPolicy policy)
    {
        Policy = policy;
    }
}