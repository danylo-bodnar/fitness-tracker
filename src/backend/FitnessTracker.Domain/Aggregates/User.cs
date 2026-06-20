namespace FitnessTracker.Domain.Aggregates;

public class User : AggregateRoot
{
    public Guid Id { get; private set; }
    public long TelegramChatId { get; private set; }
    public string? TelegramUsername { get; private set; }

    private User() { }

    public User(long telegramChatId, string? telegramUsername)
    {
        Id = Guid.NewGuid();
        TelegramChatId = telegramChatId;
        TelegramUsername = telegramUsername;
    }
}
