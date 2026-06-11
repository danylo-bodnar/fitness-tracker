using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Aggregates;

public class User(long telegramChatId, string? telegramUsername, string timezone) : AggregateRoot
{
    private User() : this(default!, default!, default!) { }

    public UserId Id { get; private set; } = new UserId(Guid.NewGuid());
    public long TelegramChatId { get; private set; } = telegramChatId;
    public string? TelegramUsername { get; private set; } = telegramUsername;
    public string Timezone { get; private set; } = timezone;
}
