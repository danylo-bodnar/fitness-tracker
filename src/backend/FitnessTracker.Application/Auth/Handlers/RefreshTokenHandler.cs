using FitnessTracker.Application.Auth.Commands;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FitnessTracker.Application.Auth.Handlers;

public sealed class RefreshTokenHandler(
    IRefreshSessionRepository refreshRepo,
    IUserRepository userRepo,
    IJwtService jwtService,
    ILogger<RefreshTokenHandler> logger)
    : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var session = await refreshRepo.GetByTokenAsync(request.RefreshToken, ct)
            ?? throw new RefreshSessionNotFoundException();

        if (session.IsRevoked)
        {
            logger.LogWarning(
                "Revoked refresh token reused for user {UserId}. Revoking all sessions.",
                session.UserId);

            await refreshRepo.RevokeAllForUserAsync(session.UserId, ct);
            throw new RefreshTokenReuseException(request.RefreshToken);
        }

        if (session.IsExpired)
            throw new RefreshSessionInvalidException();

        var user = await userRepo.GetByIdAsync(session.UserId, ct)
            ?? throw new UserNotFoundException(session.UserId);

        session.Revoke();
        await refreshRepo.UpdateAsync(session, ct);

        var newRefreshSession = RefreshSession.Create(
            user.Id,
            user.TelegramChatId,
            TimeSpan.FromDays(30));

        await refreshRepo.CreateAsync(newRefreshSession, ct);

        var newAccessToken = jwtService.GenerateToken(user.Id);

        return new RefreshTokenResult(newAccessToken, newRefreshSession.Token);
    }
}
