using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FitnessTracker.Api.Telegram;

public class CompositeUpdateHandler(
    WorkoutUpdateHandler workoutHandler,
    TelegramLoginCallbackHandler loginHandler,
    ILogger<CompositeUpdateHandler> logger)
    : IUpdateHandler
{
    private readonly ILogger<CompositeUpdateHandler> _logger = logger;

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        IUpdateHandler? handler = update.Type switch
        {
            UpdateType.CallbackQuery => loginHandler,
            UpdateType.Message => workoutHandler,
            _ => null
        };

        if (handler is null)
        {
            _logger.LogDebug("Ignored unsupported update type: {Type}", update.Type);
            return;
        }

        try
        {
            await handler.HandleUpdateAsync(bot, update, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Handler {Handler} failed", handler.GetType().Name);
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken ct)
    {
        _logger.LogError(exception, "Polling/webhook error from {Source}", source);
        await Task.CompletedTask;
    }
}