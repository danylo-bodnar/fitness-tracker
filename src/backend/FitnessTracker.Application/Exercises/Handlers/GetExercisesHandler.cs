using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Exercises.Queries;
using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Exercises.Handlers;

public sealed class GetExercisesHandler(IExerciseRepository repo)
    : IRequestHandler<GetExercisesQuery, List<ExerciseDto>>
{
    public async Task<List<ExerciseDto>> Handle(GetExercisesQuery request, CancellationToken ct)
    {
        var exercises = await repo.GetAllAsync(request.MuscleGroup, ct);

        return exercises.Select(e => new ExerciseDto(
            e.Id,
            e.Name.Value,
            e.MuscleGroup))
            .ToList();
    }
}
