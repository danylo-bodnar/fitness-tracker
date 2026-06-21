using MediatR;

namespace FitnessTracker.Application.Auth.Commands;

public record ApproveTelegramLoginCommand(
    string Nonce,
    long TelegramChatId
) : IRequest;
