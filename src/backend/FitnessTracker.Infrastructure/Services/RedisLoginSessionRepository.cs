using System.Text.Json;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Exceptions;
using StackExchange.Redis;

namespace FitnessTracker.Infrastructure.Services;

public sealed class RedisLoginSessionRepository : ILoginSessionRepository
{
    private readonly IDatabase _db;

    public RedisLoginSessionRepository(ConnectionMultiplexer multiplexer)
    {
        _db = multiplexer.GetDatabase();
    }

    public async Task CreateAsync(LoginSession session, CancellationToken ct)
    {
        var key = $"login-session:{session.Nonce}";
        var ttl = session.ExpiresAt - DateTime.UtcNow;

        if (ttl <= TimeSpan.Zero)
        {
            throw new LoginSessionExpiredException(session.Nonce);
        }

        var json = JsonSerializer.Serialize(session);
        await _db.StringSetAsync(key, json, ttl);
    }

    public async Task<LoginSession?> GetByNonceAsync(string nonce, CancellationToken ct)
    {
        var key = $"login-session:{nonce}";
        var value = await _db.StringGetAsync(key);

        return value.HasValue
            ? JsonSerializer.Deserialize<LoginSession>((string)value!)
            : null;
    }

    public async Task UpdateAsync(LoginSession session, CancellationToken ct)
    {
        var key = $"login-session:{session.Nonce}";
        var ttl = session.ExpiresAt - DateTime.UtcNow;

        if (ttl <= TimeSpan.Zero)
        {
            throw new LoginSessionExpiredException(session.Nonce);
        }

        var json = JsonSerializer.Serialize(session);
        await _db.StringSetAsync(key, json, ttl);
    }
}
