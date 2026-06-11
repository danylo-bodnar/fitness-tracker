using FitnessTracker.Domain.ValueObjects;
using MediatR;

namespace FitnessTracker.Application.Workouts.Commands;

public record LogWorkoutCommand(
    UserId UserId,
    DateOnly Date,
    ExerciseName ExerciseName,
    decimal WeightKg,
    IReadOnlyList<int> Reps
) : IRequest<SessionId>;
