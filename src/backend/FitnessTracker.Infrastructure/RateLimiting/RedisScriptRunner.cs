using StackExchange.Redis;

namespace FitnessTracker.Infrastructure.RateLimiting;

/// <summary>
/// Loads a Lua script from disk once, caches its SHA, and evaluates it via
/// EVALSHA — transparently reloading and retrying once if Redis evicts the
/// cached script (e.g. after a restart or FLUSHALL).
/// </summary>
public sealed class RedisScriptRunner
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly string _scriptSource;
    private byte[]? _scriptSha;
    private readonly SemaphoreSlim _loadLock = new(1, 1);

    public RedisScriptRunner(IConnectionMultiplexer redis, string scriptPath)
    {
        _redis = redis;
        _database = redis.GetDatabase();
        _scriptSource = File.ReadAllText(scriptPath);
    }

    public async Task<RedisResult> EvaluateAsync(RedisKey[] keys, RedisValue[] values)
    {
        try
        {
            var sha = await GetOrLoadScriptShaAsync();
            return await _database.ScriptEvaluateAsync(sha, keys, values);
        }
        catch (RedisServerException ex) when (ex.Message.StartsWith("NOSCRIPT"))
        {
            _scriptSha = null;
            var sha = await GetOrLoadScriptShaAsync();
            return await _database.ScriptEvaluateAsync(sha, keys, values);
        }
    }

    private async Task<byte[]> GetOrLoadScriptShaAsync()
    {
        if (_scriptSha is not null)
        {
            return _scriptSha;
        }

        await _loadLock.WaitAsync();
        try
        {
            if (_scriptSha is not null)
            {
                return _scriptSha;
            }

            var server = _redis.GetServer(_redis.GetEndPoints()[0]);
            _scriptSha = await server.ScriptLoadAsync(_scriptSource);
            return _scriptSha;
        }
        finally
        {
            _loadLock.Release();
        }
    }
}
