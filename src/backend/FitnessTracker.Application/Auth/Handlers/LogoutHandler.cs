using FitnessTracker.Application.Auth.Commands;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Domain.Exceptions;
using MediatR;

namespace FitnessTracker.Application.Auth.Handlers;

public sealed class LogoutHandler(IRefreshSessionRepository refreshRepo)
    : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var session = await refreshRepo.GetByTokenAsync(request.RefreshToken, ct)
            ?? throw new RefreshSessionNotFoundException();

        if (!session.IsRevoked)
        {
            session.Revoke();
            await refreshRepo.UpdateAsync(session, ct);
        }
    }
}

public sealed class LogoutEverywhereHandler(IRefreshSessionRepository refreshRepo)
    : IRequestHandler<LogoutEverywhereCommand>
{
    public async Task Handle(LogoutEverywhereCommand request, CancellationToken ct)
        => await refreshRepo.RevokeAllForUserAsync(request.UserId, ct);
}
