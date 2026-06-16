using FitnessTracker.Contracts.Events;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using FitnessTracker.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace FitnessTracker.Infrastructure.Messaging.Consumers;

public class PersonalRecordConsumer(ProjectionsDbContext db)
    : IConsumer<ExerciseLoggedEvent>
{
    public async Task Consume(ConsumeContext<ExerciseLoggedEvent> context)
    {
        var msg = context.Message;
        var estimated1RM = msg.MaxWeightKg * (1 + msg.SetCount / 30m);

        var existing = await db.UserPRs
            .FirstOrDefaultAsync(x =>
                x.UserId == msg.UserId &&
                x.ExerciseId == msg.ExerciseId);

        var isNewPR = existing is null || msg.MaxWeightKg > existing.WeightKg;
        if (!isNewPR) return;

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

        await context.Publish(new PRDetectedEvent(
            UserId: msg.UserId,
            ExerciseId: msg.ExerciseId,
            ExerciseName: msg.ExerciseName,
            WeightKg: msg.MaxWeightKg,
            Reps: msg.SetCount,
            Estimated1RM: estimated1RM,
            AchievedAt: msg.Date
        ));

        await db.SaveChangesAsync();
    }
}
