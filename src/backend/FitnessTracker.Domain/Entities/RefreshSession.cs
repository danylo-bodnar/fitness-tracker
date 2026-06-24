using System.Security.Cryptography;
using System.Text.Json.Serialization;
using FitnessTracker.Domain.Exceptions;

namespace FitnessTracker.Domain.Entities;

public sealed class RefreshSession
{
    [JsonInclude] public Guid Id { get; private set; }
    [JsonInclude] public string Token { get; private set; } = default!;
    [JsonInclude] public Guid UserId { get; private set; }
    [JsonInclude] public long TelegramChatId { get; private set; }
    [JsonInclude] public DateTime ExpiresAt { get; private set; }
    [JsonInclude] public bool IsRevoked { get; private set; }
    [JsonConstructor] private RefreshSession() { }

    public static RefreshSession Create(Guid userId, long telegramChatId, TimeSpan lifetime)
        => new()
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            TelegramChatId = telegramChatId,
            ExpiresAt = DateTime.UtcNow.Add(lifetime)
        };

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    public void Revoke()
    {
        if (IsRevoked) throw new RefreshSessionAlreadyRevokedException(Token);
        IsRevoked = true;
    }
}

