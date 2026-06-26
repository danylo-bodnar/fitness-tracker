using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Stats.Queries;

public record GetWeeklyVolumeQuery(Guid UserId, int Weeks)
    : IRequest<List<WeeklyVolumeDto>>;
