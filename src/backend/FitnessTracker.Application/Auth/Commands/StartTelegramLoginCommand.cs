using MediatR;

namespace FitnessTracker.Application.Auth.Commands;

public record StartTelegramLoginResponse(string Nonce, string TelegramLink);

public record StartTelegramLoginCommand : IRequest<StartTelegramLoginResponse>;
