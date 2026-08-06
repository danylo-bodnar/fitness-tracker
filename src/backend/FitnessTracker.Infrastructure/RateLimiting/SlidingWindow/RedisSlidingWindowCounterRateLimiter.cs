using StackExchange.Redis;

namespace FitnessTracker.Infrastructure.RateLimiting.SlidingWindow;

public sealed class RedisSlidingWindowCounterRateLimiter(RedisScriptRunner runner, SlidingWindowOptions options) : ISlidingWindowRateLimiter
{
    private readonly RedisScriptRunner _runner = runner;
    private readonly SlidingWindowOptions _options = options;

    public async Task<RateLimitResult> AllowAsync(string key)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var windowId = now / _options.WindowSeconds;
        var elapsedSeconds = now % _options.WindowSeconds;
        var elapsed = (double)elapsedSeconds / _options.WindowSeconds;

        var currentKey = $"{key}:{windowId}";
        var previousKey = $"{key}:{windowId - 1}";

        var result = await _runner.EvaluateAsync(
            keys: [currentKey, previousKey],
            values: [_options.MaxRequests, _options.WindowSeconds, elapsed]);

        var values = (RedisResult[])result!;

        return new RateLimitResult
        {
            IsAllowed = (int)values[0] == 1,
            Remaining = (int)values[1],
            CurrentCount = (int)values[2]
        };
    }
}
