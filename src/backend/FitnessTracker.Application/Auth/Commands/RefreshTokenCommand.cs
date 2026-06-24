using MediatR;

namespace FitnessTracker.Application.Auth.Commands;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResult>;

public sealed record RefreshTokenResult(string AccessToken, string RefreshToken);
