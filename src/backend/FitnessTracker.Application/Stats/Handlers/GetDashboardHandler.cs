using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Stats.Queries;
using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Stats.Handlers;

public sealed class GetDashboardHandler(IStatsRepository repo)
    : IRequestHandler<GetDashboardQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(GetDashboardQuery request, CancellationToken ct)
        => await repo.GetDashboardAsync(request.UserId, ct) ?? new DashboardStatsDto(0, 0, null);
}
