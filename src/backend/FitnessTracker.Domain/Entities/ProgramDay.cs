namespace FitnessTracker.Domain.Entities;

public class ProgramDay
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int Order { get; private set; }

    private readonly List<ProgramExercise> _exercises = new();
    public IReadOnlyList<ProgramExercise> Exercises => _exercises.AsReadOnly();

    private ProgramDay() { }

    public ProgramDay(string name, int order, List<ProgramExercise> exercises)
    {
        Id = Guid.NewGuid();
        Name = name;
        Order = order;
        _exercises = exercises ?? new List<ProgramExercise>();
    }
}