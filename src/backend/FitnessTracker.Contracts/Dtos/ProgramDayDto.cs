namespace FitnessTracker.Contracts.Dtos;

public record ProgramDayDto(
    Guid Id,
    string Name,
    IReadOnlyList<ProgramExerciseDto> Exercises
);