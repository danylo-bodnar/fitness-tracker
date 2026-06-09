using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.UnitTests.Entities;

public class SetTests
{
    [Fact]
    public void Create_SetsProperties()
    {
        var weight = new Weight(80);
        var reps = new Repetitions(6);

        var set = new Set(weight, reps);

        Assert.Equal(weight, set.Weight);
        Assert.Equal(reps, set.Repetitions);
        Assert.NotEqual(Guid.Empty, set.Id);
    }

    [Fact]
    public void Update_ModifiesWeightAndReps()
    {
        var set = new Set(new Weight(80), new Repetitions(6));

        set.Update(new Weight(100), new Repetitions(5));

        Assert.Equal(100, set.Weight.Kg);
        Assert.Equal(5, set.Repetitions.Value);
    }
}
