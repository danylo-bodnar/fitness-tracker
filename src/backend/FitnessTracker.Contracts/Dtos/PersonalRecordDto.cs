namespace FitnessTracker.Contracts.Dtos;

public record PersonalRecordDto(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    decimal WeightKg,
    int Reps,
    decimal Estimated1Rm,
    DateOnly AchievedAt
);
