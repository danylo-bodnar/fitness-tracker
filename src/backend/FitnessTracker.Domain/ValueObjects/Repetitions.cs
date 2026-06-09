using FitnessTracker.Domain.Exceptions;

namespace FitnessTracker.Domain.ValueObjects;

public record Repetitions
{
    public int Value { get; }

    public Repetitions(int value)
    {
        if (value <= 0) throw new InvalidRepetitionsException(value);
        Value = value;
    }
}
