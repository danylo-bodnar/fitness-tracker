using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.WorkoutSessions.Commands;
using FitnessTracker.Contracts.Dtos;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FitnessTracker.Api.Telegram;

public class WorkoutConversationHandler(
    IServiceScopeFactory scopeFactory,
    WorkoutStateService stateService)
{
    public async Task StartConversationAsync(ITelegramBotClient bot, long chatId, Guid userId, CancellationToken ct)
    {
        WorkoutProgramDto[] programs;

        using (var scope = scopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IWorkoutProgramReadRepository>();
            programs = [.. await repo.GetByUserAsync(userId, ct)];
        }

        if (programs.Length == 0)
        {
            await bot.SendMessage(chatId,
                "You don't have any programs yet. Create one in the web app first.",
                cancellationToken: ct);
            return;
        }

        var rows = programs.Select(p => InlineKeyboardButton.WithCallbackData(p.Name, $"program:{p.Id}"))
            .Select(b => new[] { b });

        var keyboard = new InlineKeyboardMarkup(rows);

        await stateService.SaveAsync(chatId, new WorkoutConversationState
        {
            UserId = userId,
            Step = WorkoutStep.SelectingProgram
        });

        await bot.SendMessage(chatId, "Select a program:", replyMarkup: keyboard, cancellationToken: ct);
    }

    public async Task HandleCallbackAsync(ITelegramBotClient bot, CallbackQuery callback, CancellationToken ct)
    {
        var chatId = callback.From.Id;
        var data = callback.Data;

        if (string.IsNullOrEmpty(data))
            return;

        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: ct);

        var state = await stateService.GetAsync(chatId);
        if (state is null)
        {
            await bot.SendMessage(chatId, "Session expired. Send /log to start again.", cancellationToken: ct);
            return;
        }

        if (data.StartsWith("program:") && state.Step == WorkoutStep.SelectingProgram)
        {
            await HandleProgramSelection(bot, chatId, data["program:".Length..], state, ct);
        }
        else if (data.StartsWith("day:") && state.Step == WorkoutStep.SelectingDay)
        {
            await HandleDaySelection(bot, chatId, data["day:".Length..], state, ct);
        }
        else if (data == "confirm_yes" && state.Step == WorkoutStep.Confirming)
        {
            await HandleConfirmYes(bot, chatId, state, ct);
        }
        else if (data == "confirm_no" && state.Step == WorkoutStep.Confirming)
        {
            await HandleConfirmNo(bot, chatId, state, ct);
        }
    }

    public async Task HandleTextAsync(ITelegramBotClient bot, long chatId, string text, CancellationToken ct)
    {
        var state = await stateService.GetAsync(chatId);
        if (state is null)
        {
            await bot.SendMessage(chatId,
                "Send /log to start logging a workout.", cancellationToken: ct);
            return;
        }

        switch (state.Step)
        {
            case WorkoutStep.AwaitingWeight:
                await HandleWeight(bot, chatId, text, state, ct);
                break;
            case WorkoutStep.AwaitingReps:
                await HandleReps(bot, chatId, text, state, ct);
                break;
            default:
                await bot.SendMessage(chatId, "Unexpected input. Send /log to start over.", cancellationToken: ct);
                break;
        }
    }

    private async Task HandleProgramSelection(ITelegramBotClient bot, long chatId, string programIdStr,
        WorkoutConversationState state, CancellationToken ct)
    {
        if (!Guid.TryParse(programIdStr, out var programId))
            return;

        WorkoutProgramDto program;

        using (var scope = scopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IWorkoutProgramReadRepository>();
            var programs = await repo.GetByUserAsync(state.UserId, ct);
            program = programs.FirstOrDefault(p => p.Id == programId)!;
        }

        if (program is null)
        {
            await bot.SendMessage(chatId, "Program not found.", cancellationToken: ct);
            return;
        }

        state.Step = WorkoutStep.SelectingDay;
        state.ProgramId = program.Id;
        state.ProgramName = program.Name;

        var rows = program.Days.Select(d => InlineKeyboardButton.WithCallbackData(d.Name, $"day:{d.Id}"))
            .Select(b => new[] { b });

        var keyboard = new InlineKeyboardMarkup(rows);

        await stateService.SaveAsync(chatId, state);
        await bot.SendMessage(chatId, "Select a day:", replyMarkup: keyboard, cancellationToken: ct);
    }

    private async Task HandleDaySelection(ITelegramBotClient bot, long chatId, string dayIdStr,
        WorkoutConversationState state, CancellationToken ct)
    {
        if (!Guid.TryParse(dayIdStr, out var dayId))
            return;

        WorkoutProgramDto? program;

        using (var scope = scopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IWorkoutProgramReadRepository>();
            var programs = await repo.GetByUserAsync(state.UserId, ct);
            program = programs.FirstOrDefault(p => p.Id == state.ProgramId);
        }

        var day = program?.Days.FirstOrDefault(d => d.Id == dayId);
        if (day is null)
        {
            await bot.SendMessage(chatId, "Day not found.", cancellationToken: ct);
            return;
        }

        if (day.Exercises.Count == 0)
        {
            await bot.SendMessage(chatId, "This day has no exercises.", cancellationToken: ct);
            return;
        }

        state.DayId = day.Id;
        state.DayName = day.Name;
        state.CurrentExerciseIndex = 0;
        state.CurrentExerciseSets.Clear();
        state.DayExercises = [.. day.Exercises.Select(e => new ConversationExercise
        {
            ExerciseId = e.ExerciseId,
            ExerciseName = e.ExerciseName,
            TargetSets = e.TargetSets,
            TargetReps = e.TargetReps
        })];

        var exercise = state.DayExercises[0];
        state.TotalSetsForExercise = exercise.TargetSets;
        state.CurrentSetIndex = 1;
        state.Step = WorkoutStep.AwaitingWeight;

        await stateService.SaveAsync(chatId, state);

        await bot.SendMessage(chatId,
            $"Day: {day.Name}\nExercise 1/{day.Exercises.Count}: {exercise.ExerciseName}\nEnter weight (kg) for {exercise.TargetSets} sets:",
            cancellationToken: ct);
    }

    private async Task HandleWeight(ITelegramBotClient bot, long chatId, string text,
        WorkoutConversationState state, CancellationToken ct)
    {
        if (!decimal.TryParse(text, out var weight) || weight <= 0)
        {
            await bot.SendMessage(chatId, "Please enter a valid weight (e.g. 80).", cancellationToken: ct);
            return;
        }

        state.PendingWeight = weight;
        state.Step = WorkoutStep.AwaitingReps;

        await stateService.SaveAsync(chatId, state);
        await bot.SendMessage(chatId, $"Set 1 of {state.TotalSetsForExercise} — enter reps:", cancellationToken: ct);
    }

    private async Task HandleReps(ITelegramBotClient bot, long chatId, string text,
        WorkoutConversationState state, CancellationToken ct)
    {
        if (!int.TryParse(text, out var reps) || reps <= 0)
        {
            await bot.SendMessage(chatId, "Please enter a valid number of reps (e.g. 6).", cancellationToken: ct);
            return;
        }

        state.CurrentExerciseSets.Add(new LoggedSet { WeightKg = state.PendingWeight, Reps = reps });

        if (state.CurrentSetIndex < state.TotalSetsForExercise)
        {
            state.CurrentSetIndex++;

            await stateService.SaveAsync(chatId, state);
            await bot.SendMessage(chatId,
                $"✓ {state.PendingWeight}kg × {reps}. Set {state.CurrentSetIndex} of {state.TotalSetsForExercise} — enter reps:",
                cancellationToken: ct);
        }
        else
        {
            await FinalizeCurrentExercise(bot, chatId, state, ct);
        }
    }

    private async Task FinalizeCurrentExercise(ITelegramBotClient bot, long chatId,
        WorkoutConversationState state, CancellationToken ct)
    {
        var exercise = state.DayExercises.ElementAtOrDefault(state.CurrentExerciseIndex);
        if (exercise is null)
        {
            await bot.SendMessage(chatId, "Error loading exercise data. Send /log to start over.", cancellationToken: ct);
            return;
        }

        state.CompletedExercises.Add(new CompletedExercise
        {
            ExerciseId = exercise.ExerciseId,
            ExerciseName = exercise.ExerciseName,
            Sets = [.. state.CurrentExerciseSets]
        });

        state.CurrentExerciseSets.Clear();
        state.CurrentExerciseIndex++;

        if (state.DayExercises.Count > state.CurrentExerciseIndex)
        {
            var next = state.DayExercises[state.CurrentExerciseIndex];
            state.TotalSetsForExercise = next.TargetSets;
            state.CurrentSetIndex = 1;
            state.CurrentExerciseSets.Clear();
            state.Step = WorkoutStep.AwaitingWeight;

            await stateService.SaveAsync(chatId, state);
            await bot.SendMessage(chatId,
                $"✓ {exercise.ExerciseName} done.\n\nExercise {state.CurrentExerciseIndex + 1}/{state.DayExercises.Count}: {next.ExerciseName}\nEnter weight (kg) for {next.TargetSets} sets:",
                cancellationToken: ct);
        }
        else
        {
            state.Step = WorkoutStep.Confirming;
            await stateService.SaveAsync(chatId, state);
            await ShowSummaryAndConfirm(bot, chatId, state, exercise.ExerciseName, ct);
        }
    }

    private async Task ShowSummaryAndConfirm(ITelegramBotClient bot, long chatId,
        WorkoutConversationState state, string lastExerciseName, CancellationToken ct)
    {
        var summary = $"✓ {lastExerciseName} done.\n\n";

        foreach (var ex in state.CompletedExercises)
        {
            var setsText = string.Join(", ", ex.Sets.Select(s => $"{s.WeightKg}kg×{s.Reps}"));
            summary += $"{ex.ExerciseName}: {setsText}\n";
        }

        summary += "\nLog this workout?";

        var keyboard = new InlineKeyboardMarkup([
            [InlineKeyboardButton.WithCallbackData("Yes ✅", "confirm_yes"),
             InlineKeyboardButton.WithCallbackData("No ❌", "confirm_no")]
        ]);

        await bot.SendMessage(chatId, summary, replyMarkup: keyboard, cancellationToken: ct);
    }

    private async Task HandleConfirmYes(ITelegramBotClient bot, long chatId,
        WorkoutConversationState state, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var cmd = new LogWorkoutSessionCommand(
            state.UserId,
            DateOnly.FromDateTime(DateTime.UtcNow),
            state.CompletedExercises.Select(e => new ExerciseEntry(
                e.ExerciseId,
                e.ExerciseName,
                e.Sets.Select(s => new SetEntry(s.WeightKg, s.Reps)).ToList()
            )).ToList()
        );

        await mediator.Send(cmd, ct);
        await stateService.DeleteAsync(chatId);
        await bot.SendMessage(chatId, "✅ Workout logged! Great session 💪", cancellationToken: ct);
    }

    private async Task HandleConfirmNo(ITelegramBotClient bot, long chatId,
        WorkoutConversationState state, CancellationToken ct)
    {
        await stateService.DeleteAsync(chatId);
        await bot.SendMessage(chatId, "Cancelled.", cancellationToken: ct);
    }
}
