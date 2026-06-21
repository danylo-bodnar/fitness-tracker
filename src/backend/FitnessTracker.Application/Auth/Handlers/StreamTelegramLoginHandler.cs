using System.Runtime.CompilerServices;
using FitnessTracker.Application.Auth.Commands;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Domain.Entities;
using MediatR;

namespace FitnessTracker.Application.Auth.Handlers;

public sealed class StreamTelegramLoginHandler(
    ILoginSessionRepository sessions,
    ILoginEventSubscriber loginEvents)
    : IStreamRequestHandler<StreamTelegramLoginQuery, SseEvent>
{
    public async IAsyncEnumerable<SseEvent> Handle(
        StreamTelegramLoginQuery request,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var session = await sessions.GetByNonceAsync(request.Nonce, ct);

        if (session is null)
        {
            yield return new SseEvent("login-error", "Session not found");
            yield break;
        }

        if (session.IsExpired)
        {
            yield return new SseEvent("expired", "Session expired");
            yield break;
        }

        if (session.Status == LoginSessionStatus.Approved)
        {
            yield return new SseEvent("success", new { jwt = session.Jwt });
            yield break;
        }

        yield return new SseEvent("pending", "waiting");

        var timeout = session.ExpiresAt - DateTime.UtcNow;
        var approval = await loginEvents.WaitForApprovalAsync(request.Nonce, timeout, ct);

        if (approval is null)
        {
            yield return new SseEvent("expired", "Session expired");
            yield break;
        }

        yield return new SseEvent("success", new
        {
            jwt = approval.Jwt,
            user = new
            {
                id = approval.UserId,
                telegramChatId = approval.TelegramChatId,
                telegramUsername = approval.TelegramUsername
            }
        });
    }
}