using FitnessTracker.Application.Auth.Queries;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using FitnessTracker.Domain.Exceptions;
using MediatR;

namespace FitnessTracker.Application.Auth.Handlers;

public sealed class GetCurrentUserHandler(IUserRepository userRepo)
    : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        var user = await userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new UserNotFoundException(request.UserId);

        return new UserDto(user.Id, user.TelegramChatId, user.TelegramUsername ?? "User");
    }
}
