using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.UnitTests.ValueObjects;

public class RepetitionsTests
{
    [Fact]
    public void Create_WithPositiveValue_ReturnsRepetitions()
    {
        var reps = new Repetitions(6);

        Assert.Equal(6, reps.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositiveValue_ThrowsInvalidRepetitionsException(int value)
    {
        Assert.Throws<InvalidRepetitionsException>(() => new Repetitions(value));
    }
}
