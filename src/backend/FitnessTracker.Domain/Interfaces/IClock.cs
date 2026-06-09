namespace FitnessTracker.Domain.Interfaces;

public interface IClock
{
    DateOnly Today { get; }
}
