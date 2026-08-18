using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.WorkoutSessions.Queries;
using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.WorkoutSessions.Handlers;

public sealed class GetWorkoutHistoryHandler(IWorkoutSessionReadRepository repo)
    : IRequestHandler<GetWorkoutHistoryQuery, PagedResultDto<WorkoutSessionDto>>
{
    public Task<PagedResultDto<WorkoutSessionDto>> Handle(
        GetWorkoutHistoryQuery request, CancellationToken ct)
        => repo.GetHistoryAsync(request.UserId, request.Page, request.PageSize, ct);
}