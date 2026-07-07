namespace FitnessTracker.Api.Telegram;

public enum WorkoutStep
{
    SelectingProgram,
    SelectingDay,
    AwaitingWeight,
    AwaitingReps,
    Confirming
}

public class WorkoutConversationState
{
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(3);
    public Guid UserId { get; set; }
    public WorkoutStep Step { get; set; }
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = null!;
    public Guid DayId { get; set; }
    public string DayName { get; set; } = null!;
    public int CurrentExerciseIndex { get; set; }
    public int TotalSetsForExercise { get; set; }
    public int CurrentSetIndex { get; set; }
    public decimal PendingWeight { get; set; }

    public List<ConversationExercise> DayExercises { get; set; } = [];
    public List<LoggedSet> CurrentExerciseSets { get; set; } = [];
    public List<CompletedExercise> CompletedExercises { get; set; } = [];
}

public class ConversationExercise
{
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = null!;
    public int TargetSets { get; set; }
    public int TargetReps { get; set; }
}

public class LoggedSet
{
    public decimal WeightKg { get; set; }
    public int Reps { get; set; }
}

public class CompletedExercise
{
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = null!;
    public List<LoggedSet> Sets { get; set; } = [];
}
