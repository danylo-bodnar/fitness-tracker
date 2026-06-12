using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Entities;

public class ExerciseLog
{
    private readonly List<Set> _sets = [];

    private ExerciseLog() { }

    public ExerciseLog(Guid exerciseId, ExerciseName exerciseName)
    {
        Id = Guid.NewGuid();
        ExerciseId = exerciseId;
        ExerciseName = exerciseName;
    }

    public Guid Id { get; private set; }
    public Guid ExerciseId { get; private set; }
    public ExerciseName ExerciseName { get; private set; } = default!;
    public IReadOnlyList<Set> Sets => _sets.AsReadOnly();

    public Set LogSet(Weight weight, Repetitions repetitions)
    {
        var set = new Set(weight, repetitions);
        _sets.Add(set);
        return set;
    }
}
