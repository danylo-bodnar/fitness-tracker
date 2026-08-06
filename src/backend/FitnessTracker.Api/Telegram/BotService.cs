using FitnessTracker.Application.Common.Options;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace FitnessTracker.Api.Telegram;

public class BotService(
    ITelegramBotClient bot,
    IUpdateHandler updateHandler,
    ILogger<BotService> logger,
    IOptions<TelegramOptions> telegramOptions,
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

            var webhookSecret = telegramOptions.Value.WebhookSecret;
            if (string.IsNullOrEmpty(webhookSecret))
                throw new InvalidOperationException(
                    "Telegram:WebhookSecret not set in production — cannot register webhook");

            var webhookUrl = $"https://{host}/bot";

            await bot.SetWebhook(webhookUrl, secretToken: webhookSecret, cancellationToken: ct);
            logger.LogInformation("Webhook set to {Url}", webhookUrl);
        }

        await Task.Delay(Timeout.Infinite, ct);
    }
}