using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Exercises.Queries;

public record GetExerciseQuery(Guid Id) : IRequest<ExerciseDto>;
