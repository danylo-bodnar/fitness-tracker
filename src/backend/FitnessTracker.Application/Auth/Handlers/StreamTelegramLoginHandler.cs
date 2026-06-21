using System.Runtime.CompilerServices;
using FitnessTracker.Application.Auth.Commands;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Domain.Entities;
using MediatR;

namespace FitnessTracker.Application.Auth.Handlers;

public sealed class StreamTelegramLoginHandler(
    ILoginSessionRepository sessions,
    IUserRepository users)
    : IStreamRequestHandler<StreamTelegramLoginQuery, SseEvent>
{
    public async IAsyncEnumerable<SseEvent> Handle(
        StreamTelegramLoginQuery request,
        [EnumeratorCancellation] CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var session = await sessions.GetByNonceAsync(request.Nonce, ct);

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

            if (session.Status == LoginSessionStatus.Approved)
            {
                var user = await users.GetByTelegramChatIdAsync(session.TelegramChatId!.Value, ct);

                if (user is null)
                {
                    yield return new SseEvent("error", "User not found");
                    yield break;
                }

                yield return new SseEvent("success", new
                {
                    jwt = session.Jwt,
                    user = new
                    {
                        user.Id,
                        user.TelegramChatId,
                        user.TelegramUsername
                    }
                });

                yield break;
            }

            yield return new SseEvent("pending", "waiting");
            await Task.Delay(1500, ct);
        }
    }
}