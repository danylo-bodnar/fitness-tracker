using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Common.Options;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using DomainUser = FitnessTracker.Domain.Aggregates.User;

namespace FitnessTracker.Api.Telegram;

public class WorkoutUpdateHandler(
    IServiceScopeFactory scopeFactory,
    WorkoutConversationHandler conversationHandler,
    WorkoutStateService stateService,
    ILogger<WorkoutUpdateHandler> logger,
    IOptions<AppOptions> appOptions) : IUpdateHandler
{
    private readonly string _webAppUrl = appOptions.Value.WebAppUrl;

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Message?.Text is null) return;

        var chatId = update.Message.Chat.Id;
        var text = update.Message.Text;

        using var scope = scopeFactory.CreateScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            if (text.StartsWith("/start"))
            {
                await HandleStartCommand(bot, chatId, text, update.Message.From?.Username, scope, userRepo, unitOfWork, ct);
                return;
            }

            if (text == "/log")
            {
                var user = await userRepo.GetByTelegramChatIdAsync(chatId, ct);
                if (user is null)
                {
                    await bot.SendMessage(chatId, "Please send /start first to register.", cancellationToken: ct);
                    return;
                }

                await conversationHandler.StartConversationAsync(bot, chatId, user.Id, ct);
                return;
            }

            if (text == "/help")
            {
                await bot.SendMessage(chatId,
                    "Here's what I can do:\n\n"
                    + "/log — Start logging a workout\n"
                    + "/cancel — Cancel an in-progress log\n"
                    + "/start — Show this bot's info\n\n"
                    + "During a workout entry, just type the number I ask for (weight in kg, then reps).",
                    cancellationToken: ct);
                return;
            }

            if (text == "/cancel")
            {
                if (await stateService.ExistsAsync(chatId))
                {
                    await stateService.DeleteAsync(chatId);
                    await bot.SendMessage(chatId, "Cancelled.", cancellationToken: ct);
                }
                else
                {
                    await bot.SendMessage(chatId, "Nothing to cancel.", cancellationToken: ct);
                }
                return;
            }

            if (await stateService.ExistsAsync(chatId))
            {
                await conversationHandler.HandleTextAsync(bot, chatId, text, ct);
                return;
            }

            await bot.SendMessage(chatId,
                "Send /log to log a workout, or /help for what I can do.",
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to handle message: {Text}", text);
            await bot.SendMessage(chatId,
                "❌ Something went wrong. Send /log to try again, or /cancel to reset.",
                cancellationToken: ct);
        }
    }

    private async Task HandleStartCommand(ITelegramBotClient bot, long chatId, string text,
        string? username, IServiceScope scope, IUserRepository userRepo, IUnitOfWork unitOfWork, CancellationToken ct)
    {
        var parts = text.Split(' ', 2, StringSplitOptions.TrimEntries);
        var nonce = parts.Length > 1 ? parts[1] : null;

        var existingUser = await userRepo.GetByTelegramChatIdAsync(chatId, ct);
        if (existingUser is null)
        {
            var newUser = new DomainUser(chatId, username);
            userRepo.Add(newUser);
            await unitOfWork.CommitAsync(ct);

            var programRepo = scope.ServiceProvider.GetRequiredService<IWorkoutProgramRepository>();
            var program = DefaultWorkoutProgramFactory.Create(newUser.Id);
            programRepo.Add(program);
            await unitOfWork.CommitAsync(ct);
        }

        if (nonce is not null)
        {
            var keyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("Approve login", $"login:{nonce}"));

            await bot.SendMessage(chatId, "Press the button to log in to the web app.",
                replyMarkup: keyboard, cancellationToken: ct);
        }
        else
        {
            await bot.SendMessage(chatId,
                $"Welcome to FitnessTracker! 💪\n\n"
                + $"Send /log to log a workout with interactive menus.\n\n"
                + $"View your dashboard at <a href=\"{_webAppUrl}\">{_webAppUrl}</a>",
                parseMode: ParseMode.Html, cancellationToken: ct);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken ct)
    {
        logger.LogError(exception, "Polling error from {Source}", source);
        return Task.CompletedTask;
    }
}
