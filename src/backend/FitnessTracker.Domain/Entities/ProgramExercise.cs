using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Entities;

public class ProgramExercise
{
    public Guid Id { get; private set; }
    public Guid ExerciseId { get; private set; }
    public ExerciseName ExerciseName { get; private set; } = null!;
    public int TargetSets { get; private set; }
    public int TargetReps { get; private set; }
    public int Order { get; private set; }

    private ProgramExercise() { }

    public ProgramExercise(Guid exerciseId, ExerciseName exerciseName, int targetSets, int targetReps, int order)
    {
        Id = Guid.NewGuid();
        ExerciseId = exerciseId;
        ExerciseName = exerciseName;
        TargetSets = targetSets;
        TargetReps = targetReps;
        Order = order;
    }
}