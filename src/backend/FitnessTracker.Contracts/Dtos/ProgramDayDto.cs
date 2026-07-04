namespace FitnessTracker.Contracts.Dtos;

public record ProgramDayDto(
    Guid Id,
    string Name,
    int Order,
    IReadOnlyList<ProgramExerciseDto> Exercises
);