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
    public ApprovedLoginData? ApprovedData { get; private set; }
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

    public void Approve(
        Guid userId,
        long telegramChatId,
        string telegramUsername,
        string role,
        string accessToken,
        string refreshToken)
    {
        if (IsExpired)
            throw new LoginSessionExpiredException(Nonce);

        if (Status != LoginSessionStatus.Pending)
            throw new LoginSessionAlreadyUsedException(Nonce);

        ApprovedData = new ApprovedLoginData(
            userId, telegramChatId, telegramUsername, role, accessToken, refreshToken);
        Status = LoginSessionStatus.Approved;
    }
}

public sealed record ApprovedLoginData(
    Guid UserId,
    long TelegramChatId,
    string TelegramUsername,
    string Role,
    string AccessToken,
    string RefreshToken
);

public enum LoginSessionStatus
{
    Pending,
    Approved,
    Expired
}
