namespace FitnessTracker.Domain.Aggregates;

public class User : AggregateRoot
{
    public Guid Id { get; private set; }
    public long TelegramChatId { get; private set; }
    public string? TelegramUsername { get; private set; }
    public string Timezone { get; private set; } = null!;

    private User() { }

    public User(long telegramChatId, string? telegramUsername, string timezone)
    {
        Id = Guid.NewGuid();
        TelegramChatId = telegramChatId;
        TelegramUsername = telegramUsername;
        Timezone = timezone;
    }
}