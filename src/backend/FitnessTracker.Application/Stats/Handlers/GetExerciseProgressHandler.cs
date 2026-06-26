using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Stats.Queries;
using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Stats.Handlers;

public sealed class GetExerciseProgressHandler(IStatsRepository repo)
    : IRequestHandler<GetExerciseProgressQuery, List<ExerciseProgressDto>>
{
    public Task<List<ExerciseProgressDto>> Handle(GetExerciseProgressQuery request, CancellationToken ct)
        => repo.GetExerciseProgressAsync(request.UserId, request.ExerciseId, ct);
}
