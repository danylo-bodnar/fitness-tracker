namespace FitnessTracker.Infrastructure.Persistence.ReadModels;

public class ProcessedMessage
{
    public Guid MessageId { get; set; }
    public DateTime ProcessedAt { get; set; }
}
