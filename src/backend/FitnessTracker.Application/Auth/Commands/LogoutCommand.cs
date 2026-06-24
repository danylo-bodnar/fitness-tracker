using MediatR;

namespace FitnessTracker.Application.Auth.Commands;

public sealed record LogoutCommand(string RefreshToken) : IRequest;
public sealed record LogoutEverywhereCommand(Guid UserId) : IRequest;
