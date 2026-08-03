using StackExchange.Redis;

namespace FitnessTracker.Infrastructure.RateLimiting.TokenBucket;

public sealed class RedisTokenBucketRateLimiter : ITokenBucketRateLimiter
{
    private readonly RedisScriptRunner _runner;
    private readonly TokenBucketOptions _options;

    public RedisTokenBucketRateLimiter(RedisScriptRunner runner, TokenBucketOptions options)
    {
        _runner = runner;
        _options = options;
    }

    public async Task<RateLimitResult> AllowAsync(string key)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var result = await _runner.EvaluateAsync(
            keys: [key],
            values: [_options.MaxTokens, _options.RefillRate, now]);

        var values = (RedisResult[])result!;

        return new RateLimitResult
        {
            IsAllowed = (int)values[0] == 1,
            Remaining = (int)values[1]
        };
    }
}
