namespace FitnessTracker.Infrastructure.RateLimiting.TokenBucket;

public sealed class TokenBucketOptions
{
    public const string SectionName = "RateLimiting:Api";

    public int MaxTokens { get; init; } = 15;

    public int RefillRate { get; init; } = 1;
}