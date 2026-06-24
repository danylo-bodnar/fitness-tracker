namespace FitnessTracker.Domain.Aggregates;

public class User : AggregateRoot
{
    public Guid Id { get; private set; }
    public long TelegramChatId { get; private set; }
    public string? TelegramUsername { get; private set; }
    public UserRole Role { get; private set; }

    private User() { }

    public User(long telegramChatId, string? telegramUsername)
        : this(telegramChatId, telegramUsername, UserRole.User)
    {
    }

    public User(long telegramChatId, string? telegramUsername, UserRole role)
    {
        Id = Guid.NewGuid();
        TelegramChatId = telegramChatId;
        TelegramUsername = telegramUsername;
        Role = role;
    }
}
