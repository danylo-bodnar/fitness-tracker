using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Stats.Queries;
using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Stats.Handlers;

public sealed class GetWeeklyVolumeHandler(IStatsRepository repo)
    : IRequestHandler<GetWeeklyVolumeQuery, List<WeeklyVolumeDto>>
{
    public Task<List<WeeklyVolumeDto>> Handle(GetWeeklyVolumeQuery request, CancellationToken ct)
        => repo.GetWeeklyVolumeAsync(request.UserId, request.Weeks, ct);
}
