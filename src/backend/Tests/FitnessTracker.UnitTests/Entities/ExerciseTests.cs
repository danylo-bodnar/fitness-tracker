using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.UnitTests.Entities;

public class ExerciseTests
{
    [Fact]
    public void Create_SetsProperties()
    {
        var name = new ExerciseName("bench press");
        var exercise = new Exercise(name, "Chest");

        Assert.NotEqual(Guid.Empty, exercise.Id);
        Assert.Equal(name, exercise.Name);
        Assert.Equal("Chest", exercise.MuscleGroup);
    }

    [Fact]
    public void Create_WithNullMuscleGroup_AllowsNull()
    {
        var name = new ExerciseName("squat");
        var exercise = new Exercise(name, null);

        Assert.Null(exercise.MuscleGroup);
    }
}
