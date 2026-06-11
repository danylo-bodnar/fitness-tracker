using FitnessTracker.Api.Parsers;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Domain.Interfaces;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using DomainUser = FitnessTracker.Domain.Aggregates.User;

namespace FitnessTracker.Api.Telegram;

public class WorkoutUpdateHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<WorkoutUpdateHandler> logger) : IUpdateHandler
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<WorkoutUpdateHandler> _logger = logger;

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Message?.Text is null) return;

        using var scope = _scopeFactory.CreateScope();
        var parser = scope.ServiceProvider.GetRequiredService<IWorkoutParser>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            var text = update.Message.Text;

            if (text == "/start")
            {
                var existingUser = await userRepo.GetByTelegramChatIdAsync(update.Message.Chat.Id, ct);
                if (existingUser is null)
                {
                    var newUser = new DomainUser(
                        update.Message.Chat.Id,
                        update.Message.From?.Username,
                        "UTC");

                    userRepo.Add(newUser);
                    await unitOfWork.CommitAsync(ct);
                }

                await bot.SendMessage(
                    update.Message.Chat.Id,
                    "Welcome to FitnessTracker! 💪\n\n"
                    + "Log workouts like this:\n"
                    + "bench press 80kg; 6,6,6\n\n"
                    + "View your dashboard at https://yourapp.com",
                    cancellationToken: ct);
                return;
            }

            var domainUser = await userRepo.GetByTelegramChatIdAsync(update.Message.Chat.Id, ct);
            if (domainUser is null)
            {
                await bot.SendMessage(
                    update.Message.Chat.Id,
                    "Please send /start first to register.",
                    cancellationToken: ct);
                return;
            }

            var cmd = parser.Parse(text, domainUser.Id, DateOnly.FromDateTime(DateTime.UtcNow));
            var sessionId = await mediator.Send(cmd, ct);

            await bot.SendMessage(
                update.Message.Chat.Id,
                $"Logged ✅ ({sessionId.Value})",
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to handle message: {Text}", update.Message.Text);
            await bot.SendMessage(
                update.Message.Chat.Id,
                $"❌ {ex.Message}",
                cancellationToken: ct);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken ct)
    {
        _logger.LogError(exception, "Polling error from {Source}", source);
        return Task.CompletedTask;
    }
}
