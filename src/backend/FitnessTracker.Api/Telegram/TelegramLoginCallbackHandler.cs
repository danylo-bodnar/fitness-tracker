using FitnessTracker.Application.Auth.Commands;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace FitnessTracker.Api.Telegram;

public class TelegramLoginCallbackHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<TelegramLoginCallbackHandler> logger)
    : IUpdateHandler
{
    public async Task HandleUpdateAsync(
        ITelegramBotClient bot,
        Update update,
        CancellationToken ct)
    {
        if (update.CallbackQuery is null)
            return;

        var data = update.CallbackQuery.Data;

        if (string.IsNullOrEmpty(data) || !data.StartsWith("login:"))
            return;

        var nonce = data.Split(':')[1];
        var telegramChatId = update.CallbackQuery.From.Id;

        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            await mediator.Send(new ApproveTelegramLoginCommand(nonce, telegramChatId), ct);

            await bot.AnswerCallbackQuery(
                update.CallbackQuery.Id,
                "Login approved ✅",
                cancellationToken: ct);

            await bot.SendMessage(
                telegramChatId,
                "You are now logged in. You can return to the web app.",
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Login approval failed for nonce {Nonce}", nonce);

            await bot.AnswerCallbackQuery(
                update.CallbackQuery.Id,
                "❌ Login failed or expired. Please request a new code.",
                showAlert: true,
                cancellationToken: ct);
        }
    }

    public Task HandleErrorAsync(
        ITelegramBotClient bot,
        Exception exception,
        HandleErrorSource source,
        CancellationToken ct)
    {
        logger.LogError(exception, "Callback handler error");
        return Task.CompletedTask;
    }
}