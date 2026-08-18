namespace FitnessTracker.Contracts.Dtos;

public record WorkoutSessionDto(
    Guid Id,
    DateOnly Date,
    IReadOnlyList<WorkoutExerciseDto> Exercises
);

public record WorkoutExerciseDto(
    Guid ExerciseId,
    string ExerciseName,
    IReadOnlyList<WorkoutSetDto> Sets
);

public record WorkoutSetDto(
    decimal WeightKg,
    int Reps
);
