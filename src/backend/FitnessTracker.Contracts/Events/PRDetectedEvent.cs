namespace FitnessTracker.Contracts.Events;

public record PRDetectedEvent(
    Guid UserId,
    Guid ExerciseId,
    string ExerciseName,
    decimal WeightKg,
    int Reps,
    decimal Estimated1RM,
    DateOnly AchievedAt
);
