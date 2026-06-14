using FitnessTracker.Contracts.Events;
using FitnessTracker.Domain.ValueObjects;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace FitnessTracker.Infrastructure.Messaging.Consumers;

public class NotificationConsumer(ITelegramBotClient bot, WriteDbContext db)
    : IConsumer<ExerciseLoggedEvent>, IConsumer<PRDetectedEvent>
{
    public async Task Consume(ConsumeContext<ExerciseLoggedEvent> context)
    {
        var msg = context.Message;
        var chatId = await GetChatIdAsync(msg.UserId, context.CancellationToken);
        if (chatId is null) return;

        await bot.SendMessage(
            chatId.Value,
            $"✅ {msg.ExerciseName} logged — {msg.SetCount} sets at {msg.MaxWeightKg}kg");
    }

    public async Task Consume(ConsumeContext<PRDetectedEvent> context)
    {
        var msg = context.Message;
        var chatId = await GetChatIdAsync(msg.UserId, context.CancellationToken);
        if (chatId is null) return;

        await bot.SendMessage(
            chatId.Value,
            $"🏆 New PR on {msg.ExerciseName} — {msg.WeightKg}kg! (Est. 1RM: {msg.Estimated1RM:F1}kg)");
    }

    private async Task<long?> GetChatIdAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == new UserId(userId), ct);

        return user?.TelegramChatId;
    }
}

