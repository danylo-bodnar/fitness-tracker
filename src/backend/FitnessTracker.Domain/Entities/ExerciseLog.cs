using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Entities;

public class ExerciseLog
{
    public Guid Id { get; private set; }
    public Guid ExerciseId { get; private set; }
    public ExerciseName ExerciseName { get; private set; } = null!;
    public int? SupersetGroupId { get; private set; }

    private readonly List<Set> _sets = [];
    public IReadOnlyList<Set> Sets => _sets.AsReadOnly();

    private ExerciseLog() { }

    public ExerciseLog(Guid exerciseId, ExerciseName exerciseName, int? supersetGroupId = null)
    {
        Id = Guid.NewGuid();
        ExerciseId = exerciseId;
        ExerciseName = exerciseName;
        SupersetGroupId = supersetGroupId;
    }

    public Set LogSet(Weight weight, Repetitions repetitions)
    {
        var set = new Set(weight, repetitions);
        _sets.Add(set);
        return set;
    }
}
