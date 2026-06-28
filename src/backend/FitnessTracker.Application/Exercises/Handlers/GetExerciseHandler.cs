using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Exercises.Queries;
using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Exercises.Handlers;

public sealed class GetExerciseHandler(IExerciseRepository repo)
    : IRequestHandler<GetExerciseQuery, ExerciseDto>
{
    public async Task<ExerciseDto> Handle(GetExerciseQuery request, CancellationToken ct)
    {
        var exercise = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException($"Exercise {request.Id} not found.");

        return new ExerciseDto(
            exercise.Id,
            exercise.Name.Value,
            exercise.MuscleGroup);
    }
}
