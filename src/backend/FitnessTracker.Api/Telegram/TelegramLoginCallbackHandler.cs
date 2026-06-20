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
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<TelegramLoginCallbackHandler> _logger = logger;

    public async Task HandleUpdateAsync(
        ITelegramBotClient bot,
        Update update,
        CancellationToken ct)
    {
        if (update.CallbackQuery is null)
            return;

        var data = update.CallbackQuery.Data;

        if (string.IsNullOrEmpty(data))
            return;

        if (!data.StartsWith("login:"))
            return;

        var nonce = data.Split(':')[1];
        var telegramId = update.CallbackQuery.From.Id;

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(
            new ApproveTelegramLoginCommand(nonce, telegramId),
            ct);

        await bot.AnswerCallbackQuery(
            update.CallbackQuery.Id,
            "Login approved ✅",
            cancellationToken: ct);

        await bot.SendMessage(
            telegramId,
            "You are now logged in. You can return to the web app.",
            cancellationToken: ct);
    }

    public Task HandleErrorAsync(
        ITelegramBotClient bot,
        Exception exception,
        HandleErrorSource source,
        CancellationToken ct)
    {
        _logger.LogError(exception, "Callback handler error");
        return Task.CompletedTask;
    }
}
