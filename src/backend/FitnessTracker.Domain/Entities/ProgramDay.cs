namespace FitnessTracker.Domain.Entities;

public class ProgramDay
{
    public Guid Id { get; private set; }
    public string Name { get; internal set; } = null!;
    public int Order { get; internal set; }

    private readonly List<ProgramExercise> _exercises = new();
    public IReadOnlyList<ProgramExercise> Exercises => _exercises.AsReadOnly();

    private ProgramDay() { }

    public ProgramDay(string name, int order, List<ProgramExercise> exercises)
        : this(Guid.NewGuid(), name, order, exercises)
    {
    }

    public ProgramDay(Guid id, string name, int order, List<ProgramExercise> exercises)
    {
        Id = id;
        Name = name;
        Order = order;
        _exercises = exercises ?? [];
    }

    public void ReplaceExercises(List<ProgramExercise> newExercises)
    {
        var toRemove = _exercises.Where(e => !newExercises.Any(n => n.Id == e.Id)).ToList();
        foreach (var ex in toRemove)
            _exercises.Remove(ex);

        foreach (var newEx in newExercises)
        {
            var existing = _exercises.FirstOrDefault(e => e.Id == newEx.Id);
            if (existing is not null)
            {
                existing.ExerciseId = newEx.ExerciseId;
                existing.ExerciseName = newEx.ExerciseName;
                existing.TargetSets = newEx.TargetSets;
                existing.TargetReps = newEx.TargetReps;
                existing.Order = newEx.Order;
            }
            else
            {
                _exercises.Add(newEx);
            }
        }
    }
}