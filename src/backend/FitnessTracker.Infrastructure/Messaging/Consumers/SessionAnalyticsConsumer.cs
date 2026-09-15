using FitnessTracker.Contracts.Events;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using FitnessTracker.Infrastructure.Persistence.ReadModels;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Messaging.Consumers;

/// <summary>
/// Handles session-level projections. Fires exactly once per completed workout.
/// Owns WeeklyVolume and DashboardStats — the counters that must represent the session data.
/// </summary>
public class SessionAnalyticsConsumer(ProjectionsDbContext db) : IConsumer<WorkoutSessionCompletedEvent>
{
    public async Task Consume(ConsumeContext<WorkoutSessionCompletedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var claimed = await db.Database.ExecuteSqlInterpolatedAsync(
            $"""
             INSERT INTO processed_messages (consumer_name, event_id, processed_at)
             VALUES ({nameof(SessionAnalyticsConsumer)}, {msg.EventId}, {DateTime.UtcNow})
             ON CONFLICT DO NOTHING
             """, ct);

        if (claimed == 0)
        {
            await tx.RollbackAsync(ct);
            return;
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

            // Guard against out-of-order delivery: don't let a late/replayed
            // older session event stomp a newer LastWorkoutAt.
            if (msg.Date >= stats.LastWorkoutAt)
            {
                stats.LastWorkoutAt = msg.Date;
            }

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