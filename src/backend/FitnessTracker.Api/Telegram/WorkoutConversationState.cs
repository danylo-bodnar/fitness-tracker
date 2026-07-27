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
    public decimal PendingWeight { get; set; }

    public List<ConversationGroup> Groups { get; set; } = [];
    public int CurrentGroupIndex { get; set; }
    public int CurrentRound { get; set; }
    public int CurrentExerciseInGroup { get; set; }

    public List<ExerciseAccumulator> GroupAccumulators { get; set; } = [];
    public List<CompletedExercise> CompletedExercises { get; set; } = [];
}

public class ConversationGroup
{
    public int? SupersetGroupId { get; set; }
    public int MaxRounds { get; set; }
    public List<ConversationExercise> Exercises { get; set; } = [];
}

public class ConversationExercise
{
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = null!;
    public int TargetSets { get; set; }
    public int TargetReps { get; set; }
    public decimal? AssignedWeight { get; set; }
}

public class ExerciseAccumulator
{
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = null!;
    public List<LoggedSet> Sets { get; set; } = [];
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
