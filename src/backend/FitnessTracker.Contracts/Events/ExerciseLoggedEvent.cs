namespace FitnessTracker.Contracts.Events;

public record ExerciseLoggedEvent(
    Guid UserId,
    Guid ExerciseId,
    string ExerciseName,
    DateOnly Date,
    decimal MaxWeightKg,
    decimal TotalVolume,
    int SetCount
);
