namespace FitnessTracker.Application.Common.Interfaces;

public interface ILoginEventSubscriber
{
    /// <summary>
    /// Waits for an approval event for the given nonce, or returns null if cancelled/timed out.
    /// </summary>
    Task<LoginApprovedPayload?> WaitForApprovalAsync(
        string nonce,
        TimeSpan timeout,
        CancellationToken ct);
}

public record LoginApprovedPayload(string Jwt, Guid UserId, long TelegramChatId, string? TelegramUsername);