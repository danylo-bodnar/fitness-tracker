namespace FitnessTracker.Infrastructure.Persistence.ReadModels;

public class ExerciseProgressReadModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public DateOnly WorkoutDate { get; set; }
    public decimal MaxWeightKg { get; set; }
    public decimal TotalVolume { get; set; }
    public int SetCount { get; set; }
}
