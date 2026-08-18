using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.WorkoutSessions.Queries;

public record GetWorkoutHistoryQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResultDto<WorkoutSessionDto>>;
