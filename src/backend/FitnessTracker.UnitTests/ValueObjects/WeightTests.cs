using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.UnitTests.ValueObjects;

public class WeightTests
{
    [Fact]
    public void Create_WithPositiveValue_ReturnsWeight()
    {
        var weight = new Weight(80);

        Assert.Equal(80m, weight.Kg);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositiveValue_ThrowsInvalidWeightException(decimal kg)
    {
        Assert.Throws<InvalidWeightException>(() => new Weight(kg));
    }
}
