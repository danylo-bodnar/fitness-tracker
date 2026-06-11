using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.UnitTests.Entities;

public class ExerciseLogTests
{
    [Fact]
    public void Create_SetsProperties()
    {
        var name = new ExerciseName("squat");
        var log = new ExerciseLog(name);

        Assert.Equal(name, log.Name);
        Assert.Empty(log.Sets);
        Assert.False(log.Id == Guid.Empty);
    }

    [Fact]
    public void LogSet_AddsSetToCollection()
    {
        var log = new ExerciseLog(new ExerciseName("squat"));

        log.LogSet(new Weight(100), new Repetitions(5));

        Assert.Single(log.Sets);
        Assert.Equal(100, log.Sets[0].Weight.Kg);
        Assert.Equal(5, log.Sets[0].Repetitions.Value);
    }

    [Fact]
    public void LogSet_ReturnsCreatedSet()
    {
        var log = new ExerciseLog(new ExerciseName("squat"));

        var set = log.LogSet(new Weight(100), new Repetitions(5));

        Assert.NotNull(set);
        Assert.Equal(100, set.Weight.Kg);
    }

    [Fact]
    public void LogSet_MultipleSets_AddsAll()
    {
        var log = new ExerciseLog(new ExerciseName("squat"));

        log.LogSet(new Weight(100), new Repetitions(5));
        log.LogSet(new Weight(100), new Repetitions(5));
        log.LogSet(new Weight(90), new Repetitions(6));

        Assert.Equal(3, log.Sets.Count);
    }
}
