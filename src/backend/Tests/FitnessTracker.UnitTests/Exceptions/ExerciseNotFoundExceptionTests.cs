using FitnessTracker.Domain.Exceptions;

namespace FitnessTracker.UnitTests.Exceptions;

public class ExerciseNotFoundExceptionTests
{
    [Fact]
    public void Create_SetsMessage()
    {
        var exception = new ExerciseNotFoundException("squat");

        Assert.Equal("Exercise 'squat' not found", exception.Message);
    }
}
