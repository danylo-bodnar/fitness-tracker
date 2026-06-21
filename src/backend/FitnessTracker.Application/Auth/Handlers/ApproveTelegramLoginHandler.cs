using FitnessTracker.Application.Auth.Commands;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using FitnessTracker.Domain.Exceptions;
using MediatR;

namespace FitnessTracker.Application.Auth.Handlers;

public sealed class ApproveTelegramLoginHandler(
    ILoginSessionRepository loginSessionRepository,
    ILoginEventPublisher publisher,
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

        var jwt = jwtService.GenerateToken(user.Id);

        session.Approve(request.TelegramChatId, jwt);

        await loginSessionRepository.UpdateAsync(session, cancellationToken);

        var userDto = new UserDto(user.Id, user.TelegramChatId, user.TelegramUsername ?? "User");

        await publisher.PublishApprovedAsync(session.Nonce, jwt, userDto, cancellationToken);
    }
}