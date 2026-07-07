using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FitnessTracker.Api.Telegram;

public class CompositeUpdateHandler(
    WorkoutUpdateHandler workoutHandler,
    WorkoutConversationHandler conversationHandler,
    TelegramLoginCallbackHandler loginHandler,
    ILogger<CompositeUpdateHandler> logger)
    : IUpdateHandler
{
    private readonly ILogger<CompositeUpdateHandler> _logger = logger;

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        try
        {
            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery?.Data is { } data)
            {
                if (data.StartsWith("login:"))
                {
                    await loginHandler.HandleUpdateAsync(bot, update, ct);
                }
                else
                {
                    await conversationHandler.HandleCallbackAsync(bot, update.CallbackQuery, ct);
                }
                return;
            }

            if (update.Type == UpdateType.Message)
            {
                await workoutHandler.HandleUpdateAsync(bot, update, ct);
                return;
            }

            _logger.LogDebug("Ignored unsupported update type: {Type}", update.Type);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Handler failed for update {Type}", update.Type);
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken ct)
    {
        _logger.LogError(exception, "Polling/webhook error from {Source}", source);
        await Task.CompletedTask;
    }
}