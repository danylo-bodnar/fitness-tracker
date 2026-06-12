using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Entities;

public class Exercise
{
    private Exercise() { }

    public Exercise(ExerciseName name, string? muscleGroup)
    {
        Name = name;
        MuscleGroup = muscleGroup;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public ExerciseName Name { get; } = null!;
    public string? MuscleGroup { get; }
}
