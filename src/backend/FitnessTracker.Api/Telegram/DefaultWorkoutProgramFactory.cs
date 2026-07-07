using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Api.Telegram;

public static class DefaultWorkoutProgramFactory
{
    public static WorkoutProgram Create(Guid userId)
    {
        var program = new WorkoutProgram(userId, "Push / Pull / Legs");

        program.AddDay("Upper A - Push",
        [
            new ProgramExercise(
                Guid.Parse("00000008-0000-0000-0000-000000000001"),
                new ExerciseName("bench press"), 3, 6, 1),
            new ProgramExercise(
                Guid.Parse("0000000f-0000-0000-0000-000000000001"),
                new ExerciseName("barbell row"), 3, 6, 2),
            new ProgramExercise(
                Guid.Parse("00000009-0000-0000-0000-000000000001"),
                new ExerciseName("incline dumbbell press"), 2, 8, 3),
            new ProgramExercise(
                Guid.Parse("00000001-0000-0000-0000-000000000001"),
                new ExerciseName("bicep curl"), 2, 8, 4),
            new ProgramExercise(
                Guid.Parse("0000000c-0000-0000-0000-000000000001"),
                new ExerciseName("triceps pushdown"), 2, 8, 4),
            new ProgramExercise(
                Guid.Parse("0000000b-0000-0000-0000-000000000001"),
                new ExerciseName("lateral raises"), 2, 12, 5),
        ]);

        program.AddDay("Lower",
        [
            new ProgramExercise(
                Guid.Parse("00000003-0000-0000-0000-000000000001"),
                new ExerciseName("squat"), 3, 5, 1),
            new ProgramExercise(
                Guid.Parse("00000004-0000-0000-0000-000000000001"),
                new ExerciseName("leg press"), 3, 6, 2),
            new ProgramExercise(
                Guid.Parse("00000007-0000-0000-0000-000000000001"),
                new ExerciseName("romanian deadlift"), 3, 6, 3),
            new ProgramExercise(
                Guid.Parse("00000006-0000-0000-0000-000000000001"),
                new ExerciseName("calf raises"), 3, 12, 4),
        ]);

        program.AddDay("Upper B - Pull",
        [
            new ProgramExercise(
                Guid.Parse("0000000e-0000-0000-0000-000000000001"),
                new ExerciseName("pull-ups"), 3, 6, 1),
            new ProgramExercise(
                Guid.Parse("0000000e-0000-0000-0000-000000000002"),
                new ExerciseName("t-bar row"), 3, 6, 2),
            new ProgramExercise(
                Guid.Parse("00000005-0000-0000-0000-000000000002"),
                new ExerciseName("incline bench press"), 3, 8, 3),
            new ProgramExercise(
                Guid.Parse("0000000d-0000-0000-0000-000000000001"),
                new ExerciseName("triceps extension"), 3, 8, 4),
            new ProgramExercise(
                Guid.Parse("00000002-0000-0000-0000-000000000001"),
                new ExerciseName("hammer curl"), 3, 8, 4),
            new ProgramExercise(
                Guid.Parse("0000000b-0000-0000-0000-000000000001"),
                new ExerciseName("lateral raises"), 2, 12, 5),
        ]);

        return program;
    }
}
