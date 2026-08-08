using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.Services;

namespace FitnessTracker.UnitTests.Services;

public class OneRepMaxEstimatorTests
{
    [Fact]
    public void Epley_WithWeightAndReps_AppliesFormula()
    {
        var result = OneRepMaxEstimator.Epley(100m, 3);

        Assert.Equal(110m, result);
    }

    [Fact]
    public void Epley_WithZeroWeight_ThrowsInvalidWeightException()
    {
        Assert.Throws<InvalidWeightException>(() => OneRepMaxEstimator.Epley(0m, 3));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Epley_WithNonPositiveReps_ThrowsInvalidRepetitionsException(int reps)
    {
        Assert.Throws<InvalidRepetitionsException>(() => OneRepMaxEstimator.Epley(100m, reps));
    }
}