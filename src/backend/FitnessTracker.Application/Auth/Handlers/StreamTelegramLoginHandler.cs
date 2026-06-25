using System.Runtime.CompilerServices;
using FitnessTracker.Application.Auth.Queries;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Common.Utilities;
using FitnessTracker.Contracts.Dtos;
using FitnessTracker.Domain.Entities;
using MediatR;

namespace FitnessTracker.Application.Auth.Handlers;

public sealed class StreamTelegramLoginHandler(
    ILoginSessionRepository sessionRepository,
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

        if (session.ApprovedData is null)
        {
            yield return new SseEvent("pending", "waiting");

            var timedOut = await waitTask.WaitUntilAsync(session.ExpiresAt, ct);

            if (timedOut)
            {
                yield return new SseEvent("expired", "Session expired");
                yield break;
            }

            session = await sessionRepository.GetByNonceAsync(request.Nonce, ct);
        }
        else
        {
            notifier.CancelWait(request.Nonce);
        }

        if (session?.ApprovedData is { } data)
        {
            yield return new SseEvent("success", new TelegramLoginResultDto(
                data.AccessToken,
                new UserDto(data.UserId, data.TelegramChatId, data.TelegramUsername, data.Role),
                data.RefreshToken));
        }
        else
        {
            yield return new SseEvent("expired", "Session expired");
        }
    }

}
