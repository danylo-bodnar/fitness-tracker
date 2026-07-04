using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Entities;

public class ProgramExercise
{
    public Guid Id { get; private set; }
    public Guid ExerciseId { get; internal set; }
    public ExerciseName ExerciseName { get; internal set; } = null!;
    public int TargetSets { get; internal set; }
    public int TargetReps { get; internal set; }
    public int Order { get; internal set; }

    private ProgramExercise() { }

    public ProgramExercise(Guid exerciseId, ExerciseName exerciseName, int targetSets, int targetReps, int order)
        : this(Guid.NewGuid(), exerciseId, exerciseName, targetSets, targetReps, order)
    {
    }

    public ProgramExercise(Guid id, Guid exerciseId, ExerciseName exerciseName, int targetSets, int targetReps, int order)
    {
        Id = id;
        ExerciseId = exerciseId;
        ExerciseName = exerciseName;
        TargetSets = targetSets;
        TargetReps = targetReps;
        Order = order;
    }
}