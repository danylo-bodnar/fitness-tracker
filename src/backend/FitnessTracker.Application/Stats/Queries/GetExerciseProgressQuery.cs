using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Stats.Queries;

public record GetExerciseProgressQuery(Guid UserId, Guid ExerciseId)
    : IRequest<List<ExerciseProgressDto>>;
