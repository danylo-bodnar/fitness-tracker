namespace FitnessTracker.Domain.Entities;

public class ProgramDay
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    private readonly List<ProgramExercise> _exercises = new();
    public IReadOnlyList<ProgramExercise> Exercises => _exercises.AsReadOnly();

    private ProgramDay() { }

    public ProgramDay(string name, List<ProgramExercise> exercises)
    {
        Id = Guid.NewGuid();
        Name = name;
        _exercises = exercises ?? new List<ProgramExercise>();
    }
}