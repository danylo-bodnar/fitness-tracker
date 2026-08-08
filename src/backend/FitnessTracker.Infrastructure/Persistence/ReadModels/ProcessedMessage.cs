namespace FitnessTracker.Infrastructure.Persistence.ReadModels;

public class ProcessedMessage
{
    public string ConsumerName { get; set; } = null!;
    public Guid EventId { get; set; }
    public DateTime ProcessedAt { get; set; }
}