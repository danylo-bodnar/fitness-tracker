using FitnessTracker.Domain.Exceptions;

namespace FitnessTracker.Domain.ValueObjects;

public record ExerciseName
{
    public string Value { get; }

    public ExerciseName(string value)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            throw new InvalidExerciseNameException("Exercise name cannot be empty.");
        if (trimmed.Length > 100)
            throw new InvalidExerciseNameException("Exercise name cannot exceed 100 characters.");
        Value = trimmed.ToLowerInvariant();
    }
}
