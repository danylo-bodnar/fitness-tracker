namespace FitnessTracker.Contracts.Dtos;

public record ProgramExerciseDto(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    int TargetSets,
    int TargetReps,
    int Order,
    int? SupersetGroupId = null
);
