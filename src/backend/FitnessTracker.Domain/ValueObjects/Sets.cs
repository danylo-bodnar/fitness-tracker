using FitnessTracker.Domain.Exceptions;

public record Sets
{
    public int Value { get; }

    public Sets(int value)
    {
        if (value <= 0) throw new InvalidSetsException(value);
        Value = value;
    }
}
