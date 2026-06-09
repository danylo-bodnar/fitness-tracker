using FitnessTracker.Domain.Exceptions;

namespace FitnessTracker.Domain.ValueObjects;

public record Weight
{
    public decimal Kg { get; }

    public Weight(decimal kg)
    {
        if (kg <= 0) throw new InvalidWeightException(kg);
        Kg = kg;
    }
}
