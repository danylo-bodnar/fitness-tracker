using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Exercises.Queries;

public record GetExercisesQuery(string? MuscleGroup) : IRequest<List<ExerciseDto>>;
