using FitnessTracker.Contracts.Events;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using FitnessTracker.Infrastructure.Persistence.ReadModels;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infastructure.Messaging.Consumers;

public class AnalyticsConsumer(ProjectionsDbContext db) : IConsumer<ExerciseLoggedEvent>
{
    public async Task Consume(ConsumeContext<ExerciseLoggedEvent> context)
    {
        var msg = context.Message;

        var progress = await db.ExerciseProgress
            .FirstOrDefaultAsync(x =>
                x.UserId == msg.UserId &&
                x.ExerciseId == msg.ExerciseId &&
                x.WorkoutDate == msg.Date);

        if (progress is null)
        {
            db.ExerciseProgress.Add(new ExerciseProgressReadModel
            {
                Id = Guid.NewGuid(),
                UserId = msg.UserId,
                ExerciseId = msg.ExerciseId,
                ExerciseName = msg.ExerciseName,
                WorkoutDate = msg.Date,
                MaxWeightKg = msg.MaxWeightKg,
                TotalVolume = msg.TotalVolume,
                SetCount = msg.SetCount
            });
        }
        else
        {
            progress.MaxWeightKg = Math.Max(progress.MaxWeightKg, msg.MaxWeightKg);
            progress.TotalVolume += msg.TotalVolume;
            progress.SetCount += msg.SetCount;
        }

        var weekStart = msg.Date.AddDays(-(int)msg.Date.DayOfWeek + 1);
        var weekly = await db.WeeklyVolume
            .FirstOrDefaultAsync(x =>
                x.UserId == msg.UserId &&
                x.WeekStart == weekStart);

        if (weekly is null)
        {
            db.WeeklyVolume.Add(new WeeklyVolumeReadModel
            {
                Id = Guid.NewGuid(),
                UserId = msg.UserId,
                WeekStart = weekStart,
                TotalVolume = msg.TotalVolume,
                SessionCount = 1,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            weekly.TotalVolume += msg.TotalVolume;
            weekly.SessionCount++;
            weekly.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
    }
}
