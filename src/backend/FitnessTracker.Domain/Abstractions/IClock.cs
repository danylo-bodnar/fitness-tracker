namespace FitnessTracker.Domain.Abstractions;

public interface IClock
{
    DateOnly Today { get; }
}
