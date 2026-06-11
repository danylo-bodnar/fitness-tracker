using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Entities;

public class Set(Weight weight, Repetitions repetitions)
{
    public Guid Id { get; } = Guid.NewGuid();
    public Weight Weight { get; private set; } = weight;
    public Repetitions Repetitions { get; private set; } = repetitions;

    public void Update(Weight weight, Repetitions repetitions)
    {
        Weight = weight;
        Repetitions = repetitions;
    }
}
