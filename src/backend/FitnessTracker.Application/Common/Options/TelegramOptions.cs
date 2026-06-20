namespace FitnessTracker.Application.Common.Options;

public class TelegramOptions
{
    public const string SectionName = "Telegram";

    public string BotUsername { get; set; } = default!;
}
