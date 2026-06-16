namespace FitnessTracker.Contracts.Dtos;

public record ProgramDayDto(
    string Name,
    IReadOnlyList<ProgramExerciseDto> Exercises
);