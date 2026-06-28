namespace FitnessTracker.Contracts.Dtos;

public record ExerciseDto(
    Guid Id,
    string Name,
    string? MuscleGroup
);
