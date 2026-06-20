using Telegram.Bot;
using Telegram.Bot.Polling;

namespace FitnessTracker.Api.Telegram;

public class BotService(
    ITelegramBotClient bot,
    IUpdateHandler updateHandler,
    ILogger<BotService> logger,
    IHostEnvironment env) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        if (env.IsDevelopment())
        {
            logger.LogInformation("Starting bot polling...");
            await bot.ReceiveAsync(
                updateHandler,
                new ReceiverOptions(),
                cancellationToken: ct);
        }
        else
        {
            var host = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")
                ?? throw new InvalidOperationException(
                    "WEBSITE_HOSTNAME not set — cannot register Telegram webhook");

            var webhookUrl = $"https://{host}/bot";

            await bot.SetWebhook(webhookUrl, cancellationToken: ct);
            logger.LogInformation("Webhook set to {Url}", webhookUrl);
        }

        await Task.Delay(Timeout.Infinite, ct);
    }
}