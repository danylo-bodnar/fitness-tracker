using FitnessTracker.Contracts.Dtos;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Application.WorkoutPrograms.Mappers;

public static class ProgramDayMapper
{
    public static List<ProgramDay> ToDomain(IEnumerable<ProgramDayDto> days)
        => [.. days.Select(ToProgramDay)];

    public static ProgramDay ToProgramDay(ProgramDayDto dto)
        => new(dto.Name, [.. dto.Exercises.Select(ToProgramExercise)]);

    private static ProgramExercise ToProgramExercise(ProgramExerciseDto dto)
        => new(dto.ExerciseId, new ExerciseName(dto.ExerciseName),
               dto.TargetSets, dto.TargetReps, dto.Order);
}
