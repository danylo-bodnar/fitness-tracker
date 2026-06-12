using Telegram.Bot;
using Telegram.Bot.Polling;

namespace FitnessTracker.Api.Telegram;

public class BotService(
    ITelegramBotClient bot,
    IUpdateHandler updateHandler,
    ILogger<BotService> logger,
    IHostEnvironment env) : BackgroundService
{
    private readonly ITelegramBotClient _bot = bot;
    private readonly IUpdateHandler _updateHandler = updateHandler;
    private readonly ILogger<BotService> _logger = logger;
    private readonly IHostEnvironment _env = env;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        if (_env.IsDevelopment())
        {
            _logger.LogInformation("Starting bot polling...");
            await _bot.ReceiveAsync(
                _updateHandler,
                new ReceiverOptions(),
                cancellationToken: ct);
        }
        else
        {
            var host = _env.IsProduction() ? Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME") : "localhost";
            var webhookUrl = $"https://{host}/bot";
            await _bot.SetWebhook(webhookUrl, cancellationToken: ct);
            _logger.LogInformation("Webhook set to {Url}", webhookUrl);
        }

        await Task.Delay(Timeout.Infinite, ct);
    }
}
