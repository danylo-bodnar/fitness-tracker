using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Entities;

public class ExerciseLog(ExerciseName name)
{
    private ExerciseLog() : this(default!) { }

    private readonly List<Set> _sets = [];

    public Guid Id { get; private set; } = Guid.NewGuid();
    public ExerciseName Name { get; private set; } = name;
    public IReadOnlyList<Set> Sets => _sets.AsReadOnly();

    public Set LogSet(Weight weight, Repetitions repetitions)
    {
        var set = new Set(weight, repetitions);
        _sets.Add(set);
        return set;
    }
}
