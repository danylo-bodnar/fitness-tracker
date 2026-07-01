using FitnessTracker.Api.Parsers;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Common.Options;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using DomainUser = FitnessTracker.Domain.Aggregates.User;
using WorkoutProgram = FitnessTracker.Domain.Aggregates.WorkoutProgram;

namespace FitnessTracker.Api.Telegram;

public class WorkoutUpdateHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<WorkoutUpdateHandler> logger,
    IOptions<AppOptions> appOptions) : IUpdateHandler
{
    private readonly string _webAppUrl = appOptions.Value.WebAppUrl;

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Message?.Text is null) return;

        using var scope = scopeFactory.CreateScope();
        var parser = scope.ServiceProvider.GetRequiredService<IWorkoutParser>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            var text = update.Message.Text;

            if (text.StartsWith("/start"))
            {
                var parts = text.Split(' ', 2, StringSplitOptions.TrimEntries);
                var nonce = parts.Length > 1 ? parts[1] : null;

                var existingUser = await userRepo.GetByTelegramChatIdAsync(update.Message.Chat.Id, ct);
                if (existingUser is null)
                {
                    var newUser = new DomainUser(
                        update.Message.Chat.Id,
                        update.Message.From?.Username
                        );

                    userRepo.Add(newUser);
                    await unitOfWork.CommitAsync(ct);

                    var programRepo = scope.ServiceProvider.GetRequiredService<IWorkoutProgramRepository>();

                    var program = new WorkoutProgram(newUser.Id, "Push / Pull / Legs");

                    program.AddDay("Upper A - Push",
                    [
                        new ProgramExercise(
                            Guid.Parse("00000008-0000-0000-0000-000000000001"),
                            new ExerciseName("bench press"), 3, 6, 1),
                        new ProgramExercise(
                            Guid.Parse("0000000f-0000-0000-0000-000000000001"),
                            new ExerciseName("barbell row"), 3, 6, 2),
                        new ProgramExercise(
                            Guid.Parse("00000009-0000-0000-0000-000000000001"),
                            new ExerciseName("incline dumbbell press"), 2, 8, 3),
                        new ProgramExercise(
                            Guid.Parse("00000001-0000-0000-0000-000000000001"),
                            new ExerciseName("bicep curl"), 2, 8, 4),
                        new ProgramExercise(
                            Guid.Parse("0000000c-0000-0000-0000-000000000001"),
                            new ExerciseName("triceps pushdown"), 2, 8, 4),
                        new ProgramExercise(
                            Guid.Parse("0000000b-0000-0000-0000-000000000001"),
                            new ExerciseName("lateral raises"), 2, 12, 5),
                    ]);

                    program.AddDay("Lower",
                    [
                        new ProgramExercise(
                            Guid.Parse("00000003-0000-0000-0000-000000000001"),
                            new ExerciseName("squat"), 3, 5, 1),
                        new ProgramExercise(
                            Guid.Parse("00000004-0000-0000-0000-000000000001"),
                            new ExerciseName("leg press"), 3, 6, 2),
                        new ProgramExercise(
                            Guid.Parse("00000007-0000-0000-0000-000000000001"),
                            new ExerciseName("romanian deadlift"), 3, 6, 3),
                        new ProgramExercise(
                            Guid.Parse("00000006-0000-0000-0000-000000000001"),
                            new ExerciseName("calf raises"), 3, 12, 4),
                    ]);

                    program.AddDay("Upper B - Pull",
                    [
                        new ProgramExercise(
                            Guid.Parse("0000000e-0000-0000-0000-000000000001"),
                            new ExerciseName("pull-ups"), 3, 6, 1),
                        new ProgramExercise(
                            Guid.Parse("0000000e-0000-0000-0000-000000000002"),
                            new ExerciseName("t-bar row"), 3, 6, 2),
                        new ProgramExercise(
                            Guid.Parse("00000005-0000-0000-0000-000000000002"),
                            new ExerciseName("incline bench press"), 3, 8, 3),
                        new ProgramExercise(
                            Guid.Parse("0000000d-0000-0000-0000-000000000001"),
                            new ExerciseName("triceps extension"), 3, 8, 4),
                        new ProgramExercise(
                            Guid.Parse("00000002-0000-0000-0000-000000000001"),
                            new ExerciseName("hammer curl"), 3, 8, 4),
                        new ProgramExercise(
                            Guid.Parse("0000000b-0000-0000-0000-000000000001"),
                            new ExerciseName("lateral raises"), 2, 12, 5),
                    ]);

                    programRepo.Add(program);
                    await unitOfWork.CommitAsync(ct);
                }

                if (nonce is not null)
                {
                    var keyboard = new InlineKeyboardMarkup(
                        InlineKeyboardButton.WithCallbackData("Approve login", $"login:{nonce}"));

                    await bot.SendMessage(
                        update.Message.Chat.Id,
                        "Press the button to log in to the web app.",
                        replyMarkup: keyboard,
                        cancellationToken: ct);
                }
                else
                {
                    await bot.SendMessage(
                        update.Message.Chat.Id,
                        "Welcome to FitnessTracker! 💪\n\n"
                        + "Log workouts like this:\n"
                        + "bench press 80kg; 6,6,6\n\n"
                        + $"View your dashboard at <a href=\"{_webAppUrl}\">{_webAppUrl}</a>",
                        parseMode: ParseMode.Html,
                        cancellationToken: ct);
                }

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
                $"Logged ✅ ({sessionId})",
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to handle message: {Text}", update.Message.Text);
            await bot.SendMessage(
                update.Message.Chat.Id,
                $"❌ {ex.Message}",
                cancellationToken: ct);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken ct)
    {
        logger.LogError(exception, "Polling error from {Source}", source);
        return Task.CompletedTask;
    }
}
