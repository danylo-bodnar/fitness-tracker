using FitnessTracker.Domain.ValueObjects;
using MediatR;

namespace FitnessTracker.Application.WorkoutSessions.Commands;

public record LogWorkoutCommand(
    Guid UserId,
    DateOnly Date,
    ExerciseName ExerciseName,
    decimal WeightKg,
    IReadOnlyList<int> Reps
) : IRequest<Guid>;
