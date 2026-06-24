using System.Runtime.CompilerServices;
using FitnessTracker.Application.Auth.Queries;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using FitnessTracker.Domain.Entities;
using MediatR;

namespace FitnessTracker.Application.Auth.Handlers;

public sealed class StreamTelegramLoginHandler(
    ILoginSessionRepository sessionRepository,
    IUserRepository userRepository,
    ILoginSessionNotifier notifier)
    : IStreamRequestHandler<StreamTelegramLoginQuery, SseEvent>
{
    public async IAsyncEnumerable<SseEvent> Handle(
        StreamTelegramLoginQuery request,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var session = await sessionRepository.GetByNonceAsync(request.Nonce, ct);

        if (session is null)
        {
            yield return new SseEvent("error", "Session not found");
            yield break;
        }

        if (session.IsExpired)
        {
            yield return new SseEvent("expired", "Session expired");
            yield break;
        }

        var waitTask = notifier.WaitForChangeAsync(request.Nonce, ct);
        var approval = TryGetApproval(session);

        if (approval is null)
        {
            yield return new SseEvent("pending", "waiting");

            var timedOut = await WaitWithTimeoutAsync(waitTask, session.ExpiresAt, ct);

            if (timedOut)
            {
                yield return new SseEvent("expired", "Session expired");
                yield break;
            }

            session = await sessionRepository.GetByNonceAsync(request.Nonce, ct);
            approval = session is not null ? TryGetApproval(session) : null;
        }
        else
        {
            notifier.CancelWait(request.Nonce);
        }

        if (approval is not null)
        {
            var user = await userRepository.GetByTelegramChatIdAsync(approval.TelegramChatId, ct);

            if (user is null)
            {
                yield return new SseEvent("error", "User not found");
                yield break;
            }

            yield return new SseEvent("success", new TelegramLoginResultDto(
                approval.AccessToken,
                new UserDto(user.Id, user.TelegramChatId, user.TelegramUsername ?? "User", user.Role.ToString()),
                approval.RefreshToken
            ));
        }
        else
        {
            yield return new SseEvent("expired", "Session expired");
        }
    }

    private static async Task<bool> WaitWithTimeoutAsync(
        Task waitTask,
        DateTime expiresAt,
        CancellationToken ct)
    {
        var timeout = expiresAt - DateTime.UtcNow;
        if (timeout <= TimeSpan.Zero)
        {
            return true;
        }

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(timeout);

        try
        {
            await waitTask.WaitAsync(timeoutCts.Token);
            return false;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return true;
        }
    }

    private static ApprovalInfo? TryGetApproval(LoginSession session)
    {
        if (session.Status != LoginSessionStatus.Approved)
            return null;
        if (session.TelegramChatId is null || session.AccessToken is null || session.RefreshToken is null)
            return null;

        return new ApprovalInfo(session.TelegramChatId.Value, session.AccessToken, session.RefreshToken);
    }

    private sealed record ApprovalInfo(long TelegramChatId, string AccessToken, string RefreshToken);
}

