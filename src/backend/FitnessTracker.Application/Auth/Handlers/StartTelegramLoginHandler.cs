using FitnessTracker.Application.Auth.Commands;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Common.Options;
using FitnessTracker.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace FitnessTracker.Application.Auth.Handlers;

public sealed class StartTelegramLoginHandler(
    ILoginSessionRepository loginSessionRepository,
    IOptions<TelegramOptions> telegramOptions)
    : IRequestHandler<StartTelegramLoginCommand, StartTelegramLoginResponse>
{
    public async Task<StartTelegramLoginResponse> Handle(
        StartTelegramLoginCommand request,
        CancellationToken cancellationToken)
    {
        var nonce = Guid.NewGuid().ToString();

        var session = LoginSession.Create(nonce);

        await loginSessionRepository.CreateAsync(session, cancellationToken);

        var botUsername = telegramOptions.Value.BotUsername;

        return new StartTelegramLoginResponse(
            nonce,
            $"https://t.me/{botUsername}?start={nonce}"
        );
    }
}
