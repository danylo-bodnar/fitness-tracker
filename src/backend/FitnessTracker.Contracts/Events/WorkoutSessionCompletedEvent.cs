namespace FitnessTracker.Contracts.Events;

public record WorkoutSessionCompletedEvent(
    Guid EventId,
    Guid UserId,
    Guid SessionId,
    DateOnly Date,
    decimal TotalVolume,
    int ExerciseCount
);