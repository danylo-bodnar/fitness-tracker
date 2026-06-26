namespace FitnessTracker.Contracts.Dtos;

public record ExerciseProgressDto(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    DateOnly WorkoutDate,
    decimal MaxWeightKg,
    decimal TotalVolume,
    int SetCount
);
