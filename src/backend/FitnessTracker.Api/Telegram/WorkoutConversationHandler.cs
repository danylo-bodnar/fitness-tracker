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

        var grouped = day.Exercises
            .GroupBy(e => e.SupersetGroupId)
            .OrderBy(g => day.Exercises
                .Where(e => e.SupersetGroupId == g.Key)
                .Min(e => e.Order));

        state.Groups = [];
        foreach (var grp in grouped)
        {
            var exercises = grp.Select(e => new ConversationExercise
            {
                ExerciseId = e.ExerciseId,
                ExerciseName = e.ExerciseName,
                TargetSets = e.TargetSets,
                TargetReps = e.TargetReps,
            }).ToList();

            state.Groups.Add(new ConversationGroup
            {
                SupersetGroupId = grp.Key,
                MaxRounds = exercises.Max(e => e.TargetSets),
                Exercises = exercises
            });
        }

        state.CurrentGroupIndex = 0;
        state.CurrentRound = 1;
        state.CurrentExerciseInGroup = 0;
        state.GroupAccumulators.Clear();

        await stateService.SaveAsync(chatId, state);
        await SendCurrentPrompt(bot, chatId, state, ct);
    }

    private async Task HandleWeight(ITelegramBotClient bot, long chatId, string text,
        WorkoutConversationState state, CancellationToken ct)
    {
        if (!decimal.TryParse(text, out var weight) || weight <= 0)
        {
            await bot.SendMessage(chatId, "Please enter a valid weight (e.g. 80).", cancellationToken: ct);
            return;
        }

        var group = state.Groups[state.CurrentGroupIndex];
        var exercise = group.Exercises[state.CurrentExerciseInGroup];

        state.PendingWeight = weight;
        exercise.AssignedWeight = weight;
        state.Step = WorkoutStep.AwaitingReps;

        await stateService.SaveAsync(chatId, state);
        await bot.SendMessage(chatId, $"Set {state.CurrentRound} of {exercise.TargetSets} — enter reps:", cancellationToken: ct);
    }

    private async Task HandleReps(ITelegramBotClient bot, long chatId, string text,
        WorkoutConversationState state, CancellationToken ct)
    {
        if (!int.TryParse(text, out var reps) || reps <= 0)
        {
            await bot.SendMessage(chatId, "Please enter a valid number of reps (e.g. 6).", cancellationToken: ct);
            return;
        }

        var group = state.Groups[state.CurrentGroupIndex];
        var exercise = group.Exercises[state.CurrentExerciseInGroup];

        var acc = state.GroupAccumulators.FirstOrDefault(a => a.ExerciseId == exercise.ExerciseId);
        if (acc is null)
        {
            acc = new ExerciseAccumulator
            {
                ExerciseId = exercise.ExerciseId,
                ExerciseName = exercise.ExerciseName,
            };
            state.GroupAccumulators.Add(acc);
        }

        acc.Sets.Add(new LoggedSet
        {
            WeightKg = state.PendingWeight,
            Reps = reps
        });

        await stateService.SaveAsync(chatId, state);
        await AdvanceToNextInput(bot, chatId, state, ct);
    }

    private async Task AdvanceToNextInput(ITelegramBotClient bot, long chatId,
        WorkoutConversationState state, CancellationToken ct)
    {
        var group = state.Groups[state.CurrentGroupIndex];

        // Move to next exercise in this round
        state.CurrentExerciseInGroup++;

        if (state.CurrentExerciseInGroup < group.Exercises.Count)
        {
            await SendCurrentPrompt(bot, chatId, state, ct);
            return;
        }

        // All exercises in this round done — move to next round
        state.CurrentExerciseInGroup = 0;
        state.CurrentRound++;

        if (state.CurrentRound <= group.MaxRounds)
        {
            await SendCurrentPrompt(bot, chatId, state, ct);
            return;
        }

        // All rounds done — finalize group
        await FinalizeGroup(bot, chatId, state, ct);
    }

    private async Task FinalizeGroup(ITelegramBotClient bot, long chatId,
        WorkoutConversationState state, CancellationToken ct)
    {
        var group = state.Groups[state.CurrentGroupIndex];

        foreach (var acc in state.GroupAccumulators)
        {
            state.CompletedExercises.Add(new CompletedExercise
            {
                ExerciseId = acc.ExerciseId,
                ExerciseName = acc.ExerciseName,
                Sets = [.. acc.Sets]
            });
        }

        state.GroupAccumulators.Clear();

        if (group.SupersetGroupId.HasValue && group.Exercises.Count > 1)
        {
            var names = string.Join(" + ", group.Exercises.Select(e => e.ExerciseName));
            await bot.SendMessage(chatId, $"✓ Superset complete: {names}", cancellationToken: ct);
        }

        // Move to next group
        state.CurrentGroupIndex++;
        state.CurrentRound = 1;
        state.CurrentExerciseInGroup = 0;

        if (state.CurrentGroupIndex < state.Groups.Count)
        {
            await SendCurrentPrompt(bot, chatId, state, ct);
        }
        else
        {
            state.Step = WorkoutStep.Confirming;
            await stateService.SaveAsync(chatId, state);
            await ShowSummaryAndConfirm(bot, chatId, state, ct);
        }
    }

    private async Task SendCurrentPrompt(ITelegramBotClient bot, long chatId,
        WorkoutConversationState state, CancellationToken ct)
    {
        var group = state.Groups[state.CurrentGroupIndex];
        var exercise = group.Exercises[state.CurrentExerciseInGroup];

        state.Step = exercise.AssignedWeight.HasValue
            ? WorkoutStep.AwaitingReps
            : WorkoutStep.AwaitingWeight;

        string message;

        if (group.SupersetGroupId.HasValue && group.Exercises.Count > 1)
        {
            if (exercise.AssignedWeight.HasValue)
            {
                message = $"Round {state.CurrentRound}/{group.MaxRounds} — {exercise.ExerciseName} — Set {state.CurrentRound} — enter reps:";
            }
            else
            {
                message = $"Round {state.CurrentRound}/{group.MaxRounds} — {exercise.ExerciseName} — Enter weight (kg) for {exercise.TargetSets} sets:";
            }
        }
        else
        {
            if (exercise.AssignedWeight.HasValue)
            {
                message = $"Set {state.CurrentRound} of {exercise.TargetSets} — enter reps:";
            }
            else
            {
                var dayProgress = state.Groups
                    .TakeWhile(g => g != group)
                    .Sum(g => g.Exercises.Count);

                var total = state.Groups.Sum(g => g.Exercises.Count);
                var exerciseNum = dayProgress + state.CurrentExerciseInGroup + 1;

                message = $"Day: {state.DayName}\nExercise {exerciseNum}/{total}: {exercise.ExerciseName}\nEnter weight (kg) for {exercise.TargetSets} sets:";
            }
        }

        await stateService.SaveAsync(chatId, state);
        await bot.SendMessage(chatId, message, cancellationToken: ct);
    }

    private async Task ShowSummaryAndConfirm(ITelegramBotClient bot, long chatId,
        WorkoutConversationState state, CancellationToken ct)
    {
        var summary = "";

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
