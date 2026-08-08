namespace FitnessTracker.Contracts.Events;

public record ExerciseLoggedEvent(
    Guid EventId,
    Guid UserId,
    Guid ExerciseId,
    string ExerciseName,
    DateOnly Date,
    decimal MaxWeightKg,
    decimal Estimated1Rm,
    int BestSetReps,
    decimal TotalVolume,
    int SetCount,
    int? SupersetGroupId = null
);
