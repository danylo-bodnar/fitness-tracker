using FitnessTracker.Contracts.Events;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using FitnessTracker.Infrastructure.Persistence.ReadModels;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Messaging.Consumers;

public class PersonalRecordConsumer(ProjectionsDbContext db)
    : IConsumer<ExerciseLoggedEvent>
{
    public async Task Consume(ConsumeContext<ExerciseLoggedEvent> context)
    {
        var msg = context.Message;

        await using var tx = await db.Database.BeginTransactionAsync(context.CancellationToken);

        var claimed = await db.Database.ExecuteSqlInterpolatedAsync(
            $"""
             INSERT INTO processed_messages (consumer_name, event_id, processed_at)
             VALUES ({nameof(PersonalRecordConsumer)}, {msg.EventId}, {DateTime.UtcNow})
             ON CONFLICT DO NOTHING
             """, context.CancellationToken);

        if (claimed == 0)
        {
            await tx.RollbackAsync(context.CancellationToken);
            return;
        }

        //TODO: adjust this formula
        var estimated1RM = msg.MaxWeightKg * (1 + msg.SetCount / 30m);

        var existing = await db.UserPRs
            .FirstOrDefaultAsync(x =>
                x.UserId == msg.UserId &&
                x.ExerciseId == msg.ExerciseId);

        var isNewPR = existing is null || msg.MaxWeightKg > existing.WeightKg;
        if (!isNewPR)
        {
            await tx.CommitAsync(context.CancellationToken);
            return;
        }

        if (existing is null)
        {
            db.UserPRs.Add(new PersonalRecordReadModel
            {
                Id = Guid.NewGuid(),
                UserId = msg.UserId,
                ExerciseId = msg.ExerciseId,
                ExerciseName = msg.ExerciseName,
                WeightKg = msg.MaxWeightKg,
                Reps = msg.SetCount,
                Estimated1RM = estimated1RM,
                AchievedAt = msg.Date
            });
        }
        else
        {
            existing.WeightKg = msg.MaxWeightKg;
            existing.Estimated1RM = estimated1RM;
            existing.AchievedAt = msg.Date;
        }

        await db.SaveChangesAsync(context.CancellationToken);

        await context.Publish(new PRDetectedEvent(
            EventId: msg.EventId,
            UserId: msg.UserId,
            ExerciseId: msg.ExerciseId,
            ExerciseName: msg.ExerciseName,
            WeightKg: msg.MaxWeightKg,
            Reps: msg.SetCount,
            Estimated1RM: estimated1RM,
            AchievedAt: msg.Date
        ));

        await tx.CommitAsync(context.CancellationToken);
    }
}
