using System.Text.Json.Serialization;

namespace FitnessTracker.Domain.Entities;

using FitnessTracker.Domain.Exceptions;

public sealed class LoginSession
{
    [JsonInclude]
    public Guid Id { get; private set; }
    [JsonInclude]
    public string Nonce { get; private set; } = default!;
    [JsonInclude]
    public LoginSessionStatus Status { get; private set; } = LoginSessionStatus.Pending;
    [JsonInclude]
    public string? AccessToken { get; private set; }
    [JsonInclude]
    public long? TelegramChatId { get; private set; }
    [JsonInclude]
    public string? RefreshToken { get; private set; }
    [JsonInclude]
    public DateTime ExpiresAt { get; private set; }

    [JsonConstructor]
    private LoginSession() { }

    private LoginSession(string nonce, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        Nonce = nonce;
        ExpiresAt = expiresAt;
    }

    public static LoginSession Create(string nonce)
        => new(nonce, DateTime.UtcNow.AddMinutes(5));

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    public void Approve(long telegramChatId, string accessToken, string refreshToekn)
    {
        if (IsExpired)
        {
            throw new LoginSessionExpiredException(Nonce);
        }

        if (Status != LoginSessionStatus.Pending)
        {
            throw new LoginSessionAlreadyUsedException(Nonce);
        }

        TelegramChatId = telegramChatId;
        AccessToken = accessToken;
        RefreshToken = refreshToekn;
        Status = LoginSessionStatus.Approved;
    }
}

public enum LoginSessionStatus
{
    Pending,
    Approved,
    Expired
}
