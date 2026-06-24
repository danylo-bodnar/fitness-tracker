using System.Security.Cryptography;
using FitnessTracker.Application.Common.Interfaces;
using StackExchange.Redis;

namespace FitnessTracker.Infrastructure.Services;

public sealed class RedisAuthCodeStore : IAuthCodeStore
{
    private readonly IDatabase _db;
    private static readonly TimeSpan CodeTtl = TimeSpan.FromSeconds(30);

    public RedisAuthCodeStore(ConnectionMultiplexer multiplexer)
        => _db = multiplexer.GetDatabase();

    public string Store(string refreshToken)
    {
        var code = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-").Replace("/", "_").TrimEnd('=');

        _db.StringSet($"auth-code:{code}", refreshToken, CodeTtl);
        return code;
    }

    public string? Consume(string code)
    {
        var key = $"auth-code:{code}";

        var value = (string?)_db.StringGetDelete(key);
        return value;
    }
}