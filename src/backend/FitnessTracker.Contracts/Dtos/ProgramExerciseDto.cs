namespace FitnessTracker.Contracts.Dtos;

public record ProgramExerciseDto(
    Guid Id,
    string ExerciseName,
    int TargetSets,
    int TargetReps,
    int Order
);
