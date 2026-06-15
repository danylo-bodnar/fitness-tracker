namespace FitnessTracker.Domain.Aggregates;

public class User(long telegramChatId, string? telegramUsername, string timezone) : AggregateRoot
{
    private User() : this(default!, default!, default!) { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public long TelegramChatId { get; private set; } = telegramChatId;
    public string? TelegramUsername { get; private set; } = telegramUsername;
    public string Timezone { get; private set; } = timezone;
}
