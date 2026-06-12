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
    public void Create_NormalizesToLowerCase()
    {
        var name = new ExerciseName("Bench Press");

        Assert.Equal("bench press", name.Value);
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
    public void Create_TrimsWhitespace()
    {
        var name = new ExerciseName("  squat  ");

        Assert.Equal("squat", name.Value);
    }

    [Fact]
    public void Create_WithMaxLengthName_ReturnsExerciseName()
    {
        var longName = new string('a', 100);

        var name = new ExerciseName(longName);

        Assert.Equal(longName, name.Value);
    }

    [Fact]
    public void Create_WithExcessLengthName_ThrowsInvalidExerciseNameException()
    {
        Assert.Throws<InvalidExerciseNameException>(() => new ExerciseName(new string('a', 101)));
    }
}
