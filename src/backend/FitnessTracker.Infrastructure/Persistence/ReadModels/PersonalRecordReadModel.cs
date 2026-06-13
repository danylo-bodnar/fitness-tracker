namespace FitnessTracker.Infrastructure.Persistence.ReadModels;

public class PersonalRecordReadModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public int Reps { get; set; }
    public decimal Estimated1RM { get; set; }
    public DateOnly AchievedAt { get; set; }
}
