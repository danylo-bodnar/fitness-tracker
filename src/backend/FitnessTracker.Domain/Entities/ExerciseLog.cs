using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Entities;

public class ExerciseLog
{
    public Guid Id { get; private set; }
    public Guid ExerciseId { get; private set; }
    public ExerciseName ExerciseName { get; private set; } = null!;

    private readonly List<Set> _sets = [];
    public IReadOnlyList<Set> Sets => _sets.AsReadOnly();

    private ExerciseLog() { }

    public ExerciseLog(Guid exerciseId, ExerciseName exerciseName)
    {
        Id = Guid.NewGuid();
        ExerciseId = exerciseId;
        ExerciseName = exerciseName;
    }

    public Set LogSet(Weight weight, Repetitions repetitions)
    {
        var set = new Set(weight, repetitions);
        _sets.Add(set);
        return set;
    }
}
