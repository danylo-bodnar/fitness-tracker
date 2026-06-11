using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.UnitTests.ValueObjects;

public class ExerciseNameTests
{
    [Fact]
    public void Create_WithValidName_ReturnsExerciseName()
    {
        var name = new ExerciseName("squat");

        Assert.Equal("squat", name.Value);
    }

    [Fact]
    public void Create_IsCaseInsensitive()
    {
        var name = new ExerciseName("Bench Press");

        Assert.Equal("Bench Press", name.Value);
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsInvalidExerciseNameException()
    {
        Assert.Throws<InvalidExerciseNameException>(() => new ExerciseName(""));
    }

    [Fact]
    public void Create_WithWhitespaceName_ThrowsInvalidExerciseNameException()
    {
        Assert.Throws<InvalidExerciseNameException>(() => new ExerciseName("  "));
    }

    [Fact]
    public void Create_WithUnrecognizedName_ThrowsInvalidExerciseNameException()
    {
        Assert.Throws<InvalidExerciseNameException>(() => new ExerciseName("deadlift"));
    }
}
