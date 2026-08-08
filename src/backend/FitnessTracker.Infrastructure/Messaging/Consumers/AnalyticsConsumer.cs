using FitnessTracker.Contracts.Events;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using FitnessTracker.Infrastructure.Persistence.ReadModels;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Messaging.Consumers;

public class AnalyticsConsumer(ProjectionsDbContext db) : IConsumer<ExerciseLoggedEvent>
{
    public async Task Consume(ConsumeContext<ExerciseLoggedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var claimed = await db.Database.ExecuteSqlInterpolatedAsync(
            $"""
             INSERT INTO processed_messages (consumer_name, event_id, processed_at)
             VALUES ({nameof(AnalyticsConsumer)}, {msg.EventId}, {DateTime.UtcNow})
             ON CONFLICT DO NOTHING
             """, ct);

        if (claimed == 0)
        {
            await tx.RollbackAsync(ct);
            return;
        }

        var progress = await db.ExerciseProgress
            .FirstOrDefaultAsync(x =>
                x.UserId == msg.UserId &&
                x.ExerciseId == msg.ExerciseId &&
                x.WorkoutDate == msg.Date, ct);

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
            progress.ExerciseName = msg.ExerciseName;
            progress.MaxWeightKg = Math.Max(progress.MaxWeightKg, msg.MaxWeightKg);
            progress.TotalVolume += msg.TotalVolume;
            progress.SetCount += msg.SetCount;
        }

        var weekStart = GetWeekStart(msg.Date);
        var weekly = await db.WeeklyVolume
            .FirstOrDefaultAsync(x =>
                x.UserId == msg.UserId &&
                x.WeekStart == weekStart, ct);

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

        var stats = await db.DashboardStats
            .FirstOrDefaultAsync(x => x.UserId == msg.UserId, ct);

        if (stats is null)
        {
            db.DashboardStats.Add(new DashboardStatsReadModel
            {
                UserId = msg.UserId,
                TotalSessions = 1,
                TotalVolumeKg = msg.TotalVolume,
                LastWorkoutAt = msg.Date,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            stats.TotalSessions++;
            stats.TotalVolumeKg += msg.TotalVolume;
            stats.LastWorkoutAt = msg.Date;
            stats.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    private static DateOnly GetWeekStart(DateOnly date)
    {
        var diff = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return date.AddDays(-diff);
    }
}
