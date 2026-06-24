using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Auth.Queries;

public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto>;
