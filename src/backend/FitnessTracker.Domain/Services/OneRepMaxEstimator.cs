using FitnessTracker.Domain.Exceptions;

namespace FitnessTracker.Domain.Services;

public static class OneRepMaxEstimator
{
    public static decimal Epley(decimal weightKg, int reps)
    {
        if (weightKg <= 0) throw new InvalidWeightException(weightKg);
        if (reps <= 0) throw new InvalidRepetitionsException(reps);

        return weightKg * (1 + reps / 30m);
    }
}