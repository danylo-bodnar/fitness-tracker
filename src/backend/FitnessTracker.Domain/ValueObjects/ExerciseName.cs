using FitnessTracker.Domain.Exceptions;

namespace FitnessTracker.Domain.ValueObjects;

public record ExerciseName
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        "bicep curl",
        "hammer curl",

        "squat",
        "leg press",
        "leg curl",
        "calf raises",
        "romanian deadlift",

        "bench press",
        "incline dumbbell press",
        "dips",
        "lateral raises",

        "triceps pushdown",
        "triceps extension",

        "pull-ups",

        "barbell row",
        "cable row",
        "machine row",
    };

    public string Value { get; }

    public ExerciseName(string value)
    {
        var trimmed = value?.Trim();

        if (string.IsNullOrWhiteSpace(trimmed))
            throw new InvalidExerciseNameException("Exercise name cannot be empty.");

        if (!Allowed.Contains(trimmed))
            throw new InvalidExerciseNameException($"'{value}' is not a recognized exercise.");

        Value = trimmed;
    }
}
