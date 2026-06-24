using FitnessTracker.Application.Auth.Commands;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Exceptions;
using MediatR;

namespace FitnessTracker.Application.Auth.Handlers;

public sealed class ApproveTelegramLoginHandler(
    ILoginSessionRepository loginSessionRepository,
    ILoginSessionNotifier notifier,
    IRefreshSessionRepository refreshSessionRepository,
    IUserRepository userRepository,
    IJwtService jwtService)
    : IRequestHandler<ApproveTelegramLoginCommand>
{
    public async Task Handle(ApproveTelegramLoginCommand request, CancellationToken cancellationToken)
    {
        var session = await loginSessionRepository.GetByNonceAsync(request.Nonce, cancellationToken)
            ?? throw new LoginSessionNotFoundException(request.Nonce);

        var user = await userRepository.GetByTelegramChatIdAsync(request.TelegramChatId, cancellationToken)
            ?? throw new UserNotFoundException(request.TelegramChatId);

        var accessToken = jwtService.GenerateToken(user.Id);

        var refreshSession = RefreshSession.Create(user.Id, user.TelegramChatId, TimeSpan.FromDays(30));
        await refreshSessionRepository.CreateAsync(refreshSession, cancellationToken);

        session.Approve(request.TelegramChatId, accessToken, refreshSession.Token);

        await loginSessionRepository.UpdateAsync(session, cancellationToken);

        notifier.NotifyChanged(session.Nonce);
    }
}
