using FitnessTracker.Domain.Abstractions;

namespace FitnessTracker.Domain.Events;

public record WorkoutSessionCompleted(
    Guid EventId,
    Guid SessionId,
    Guid UserId,
    DateOnly Date,
    decimal TotalVolume,
    int ExerciseCount
) : IDomainEvent;