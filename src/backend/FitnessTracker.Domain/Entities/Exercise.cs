using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Entities;

public class Exercise
{
    public Guid Id { get; init; }
    public ExerciseName Name { get; init; } = null!;
    public string? MuscleGroup { get; init; }

    private Exercise() { }

    public Exercise(ExerciseName name, string? muscleGroup)
    {
        Id = Guid.NewGuid();
        Name = name;
        MuscleGroup = muscleGroup;
    }
}
