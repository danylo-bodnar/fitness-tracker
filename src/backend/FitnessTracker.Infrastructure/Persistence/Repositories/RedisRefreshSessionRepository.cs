using System.Text.Json;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Exceptions;
using StackExchange.Redis;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public sealed class RedisRefreshSessionRepository(ConnectionMultiplexer multiplexer)
    : IRefreshSessionRepository
{
    private readonly IDatabase _db = multiplexer.GetDatabase();

    public async Task CreateAsync(RefreshSession session, CancellationToken ct)
    {
        var key = SessionKey(session.Token);
        var ttl = session.ExpiresAt - DateTime.UtcNow;
        if (ttl <= TimeSpan.Zero) throw new RefreshSessionInvalidException();

        var json = JsonSerializer.Serialize(session);

        var tx = _db.CreateTransaction();
        _ = tx.StringSetAsync(key, json, ttl);
        _ = tx.SetAddAsync(UserIndexKey(session.UserId), session.Token);
        _ = tx.KeyExpireAsync(UserIndexKey(session.UserId), TimeSpan.FromDays(31));
        await tx.ExecuteAsync();
    }

    public async Task<RefreshSession?> GetByTokenAsync(string token, CancellationToken ct)
    {
        var value = await _db.StringGetAsync(SessionKey(token));
        return value.HasValue
            ? JsonSerializer.Deserialize<RefreshSession>((string)value!)
            : null;
    }

    public async Task UpdateAsync(RefreshSession session, CancellationToken ct)
    {
        var key = SessionKey(session.Token);
        var ttl = session.ExpiresAt - DateTime.UtcNow;

        var effectiveTtl = ttl > TimeSpan.Zero ? ttl : TimeSpan.FromMinutes(5);

        var json = JsonSerializer.Serialize(session);
        await _db.StringSetAsync(key, json, effectiveTtl);
    }

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct)
    {
        var indexKey = UserIndexKey(userId);
        var tokens = await _db.SetMembersAsync(indexKey);

        if (tokens.Length == 0) return;

        var tx = _db.CreateTransaction();
        foreach (var token in tokens)
        {
            var sessionKey = SessionKey(token!);
            var value = await _db.StringGetAsync(sessionKey);
            if (!value.HasValue) continue;

            var session = JsonSerializer.Deserialize<RefreshSession>((string)value!);
            if (session is null || session.IsRevoked) continue;

            session.Revoke();
            var json = JsonSerializer.Serialize(session);
            _ = tx.StringSetAsync(sessionKey, json, TimeSpan.FromMinutes(5));
        }
        _ = tx.KeyDeleteAsync(indexKey);
        await tx.ExecuteAsync();
    }

    private static string SessionKey(string token) => $"refresh-session:{token}";
    private static string UserIndexKey(Guid userId) => $"refresh-sessions:user:{userId}";
}
