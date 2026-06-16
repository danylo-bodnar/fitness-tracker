namespace FitnessTracker.Contracts.Dtos;

public record ProgramExerciseDto(
    Guid ExerciseId,
    string ExerciseName,
    int TargetSets,
    int TargetReps,
    int Order
);
