using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace FitnessTracker.Api.Telegram;

public class CompositeUpdateHandler(
    WorkoutUpdateHandler workoutHandler,
    TelegramLoginCallbackHandler loginHandler,
    ILogger<CompositeUpdateHandler> logger)
    : IUpdateHandler
{
    private readonly IUpdateHandler[] _handlers = [workoutHandler, loginHandler];
    private readonly ILogger<CompositeUpdateHandler> _logger = logger;

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        foreach (var handler in _handlers)
        {
            try
            {
                await handler.HandleUpdateAsync(bot, update, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Handler {Handler} failed", handler.GetType().Name);
            }
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken ct)
    {
        foreach (var handler in _handlers)
        {
            try
            {
                await handler.HandleErrorAsync(bot, exception, source, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error handler {Handler} failed", handler.GetType().Name);
            }
        }
    }
}
