using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Exercises.Queries;
using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Exercises.Handlers;

public sealed class GetExercisesHandler(IExerciseReadRepository exerciseRepo)
    : IRequestHandler<GetExercisesQuery, List<ExerciseDto>>
{
    public async Task<List<ExerciseDto>> Handle(GetExercisesQuery request, CancellationToken ct)
    {
        return request.MuscleGroup is null
            ? await exerciseRepo.GetAllDefaultAsync(ct)
            : await exerciseRepo.SearchAsync(request.MuscleGroup, ct);
    }
}
