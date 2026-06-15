using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Entities;

public class Set
{
    public Guid Id { get; private set; }
    public Weight Weight { get; private set; } = null!;
    public Repetitions Repetitions { get; private set; } = null!;

    private Set() { }

    public Set(Weight weight, Repetitions repetitions)
    {
        Id = Guid.NewGuid();
        Weight = weight;
        Repetitions = repetitions;
    }

    public void Update(Weight weight, Repetitions repetitions)
    {
        Weight = weight;
        Repetitions = repetitions;
    }
}