using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Services;
using FitnessTracker.Domain.ValueObjects;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using FitnessTracker.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Migrator;

public static class SeedData
{
    private static readonly IReadOnlyDictionary<string, Guid> ExerciseIds = new Dictionary<string, Guid>
    {
        ["squat"] = new("00000003-0000-0000-0000-000000000001"),
        ["leg press"] = new("00000004-0000-0000-0000-000000000001"),
        ["bench press"] = new("00000008-0000-0000-0000-000000000001"),
        ["incline bench press"] = new("00000005-0000-0000-0000-000000000002"),
        ["incline dumbbell press"] = new("00000009-0000-0000-0000-000000000001"),
        ["barbell row"] = new("0000000f-0000-0000-0000-000000000001"),
        ["machine row"] = new("00000011-0000-0000-0000-000000000001"),
        ["pull-ups"] = new("0000000e-0000-0000-0000-000000000001"),
        ["triceps pushdown"] = new("0000000c-0000-0000-0000-000000000001"),
        ["bicep curl"] = new("00000001-0000-0000-0000-000000000001")
    };

    private sealed record ExerciseSeed(string Name, decimal WeightKg, int[] Reps);
    private sealed record SessionSeed(params ExerciseSeed[] Exercises);

    private static readonly SessionSeed[] Sessions =
    [
        new(new ExerciseSeed("squat", 80, [6, 6, 5]),
            new ExerciseSeed("leg press", 50, [5, 5, 4])),

        new(new ExerciseSeed("machine row", 75, [8, 7, 6]),
            new ExerciseSeed("incline bench press", 70, [7, 6, 6])),

        new(new ExerciseSeed("bench press", 80, [8, 7, 6]),
            new ExerciseSeed("barbell row", 100, [10, 8, 7])),

        new(new ExerciseSeed("squat", 80, [7, 6, 5]),
            new ExerciseSeed("leg press", 50, [8, 6, 5])),

        new(new ExerciseSeed("pull-ups", 12, [7, 6, 5]),
            new ExerciseSeed("incline bench press", 70, [9, 7, 7]),
            new ExerciseSeed("machine row", 75, [10, 10, 8]),
            new ExerciseSeed("triceps pushdown", 84, [9, 9, 8]),
            new ExerciseSeed("bicep curl", 20, [6, 6, 6])),

        new(new ExerciseSeed("bench press", 80, [9, 8, 7]),
            new ExerciseSeed("barbell row", 100, [6, 5])),

        new(new ExerciseSeed("squat", 80, [8, 7, 6]),
            new ExerciseSeed("leg press", 50, [6, 6, 7])),

        new(new ExerciseSeed("pull-ups", 12, [8, 8, 7]),
            new ExerciseSeed("machine row", 80, [7, 7, 6]),
            new ExerciseSeed("incline dumbbell press", 70, [9, 10, 6])),

        new(new ExerciseSeed("bench press", 80, [9, 8, 7]),
            new ExerciseSeed("bicep curl", 20, [7, 7])),

        new(new ExerciseSeed("pull-ups", 12, [7, 7, 6]),
            new ExerciseSeed("incline bench press", 75, [8, 6]),
            new ExerciseSeed("machine row", 80, [8, 7, 6])),

        new(new ExerciseSeed("bench press", 80, [8, 8, 7]),
            new ExerciseSeed("barbell row", 100, [6, 5]))
    ];

    public static async Task SeedAsync(
        AppDbContext appDb,
        ProjectionsDbContext projectionsDb,
        long telegramChatId,
        string telegramUsername,
        CancellationToken ct)
    {
        var existing = await appDb.Users.FirstOrDefaultAsync(u => u.TelegramChatId == telegramChatId, ct);
        var user = existing ?? new User(telegramChatId, telegramUsername);
        var createdUser = existing is null;
        if (createdUser)
        {
            appDb.Users.Add(user);
        }

        var hasSessions = await appDb.WorkoutSessions.AnyAsync(s => s.UserId == user.Id, ct);

        var loggedExercises = new List<(DateOnly Date, ExerciseLog Log)>();
        var addedSessions = new List<WorkoutSession>();

        if (!hasSessions)
        {
            var dates = GenerateSessionDates(Sessions.Length);
            for (var i = 0; i < Sessions.Length; i++)
            {
                var sessionSeed = Sessions[i];
                var session = WorkoutSession.Create(user.Id, dates[i]);

                foreach (var exerciseSeed in sessionSeed.Exercises)
                {
                    if (!ExerciseIds.TryGetValue(exerciseSeed.Name, out var exerciseId))
                        throw new InvalidOperationException($"No seeded exercise matches '{exerciseSeed.Name}'");

                    var exerciseLog = session.AddExercise(exerciseId, new ExerciseName(exerciseSeed.Name));
                    foreach (var reps in exerciseSeed.Reps)
                        exerciseLog.LogSet(new Weight(exerciseSeed.WeightKg), new Repetitions(reps));
                    session.CompleteExercise(exerciseLog);

                    loggedExercises.Add((dates[i], exerciseLog));
                }

                appDb.WorkoutSessions.Add(session);
                addedSessions.Add(session);
            }
        }
        else
        {
            var sessions = await appDb.WorkoutSessions
                .Include(s => s.Exercises)
                .ThenInclude(e => e.Sets)
                .Where(s => s.UserId == user.Id)
                .OrderBy(s => s.Date)
                .ToListAsync(ct);

            loggedExercises = sessions
                .SelectMany(s => s.Exercises.Select(e => (Date: s.Date, Log: e)))
                .ToList();
        }

        await RebuildProjectionsAsync(projectionsDb, user.Id, loggedExercises, ct);

        await appDb.SaveChangesAsync(ct);

        try
        {
            await projectionsDb.SaveChangesAsync(ct);
        }
        catch
        {
            appDb.WorkoutSessions.RemoveRange(addedSessions);
            if (createdUser)
                appDb.Users.Remove(user);
            await appDb.SaveChangesAsync(ct);
            throw;
        }

        Console.WriteLine($"✅ Dev seed data inserted: {loggedExercises.Count} exercise logs across {Sessions.Length} sessions for user {telegramChatId}");
    }

    private static async Task RebuildProjectionsAsync(
        ProjectionsDbContext projectionsDb,
        Guid userId,
        List<(DateOnly Date, ExerciseLog Log)> loggedExercises,
        CancellationToken ct)
    {
        await using var tx = await projectionsDb.Database.BeginTransactionAsync(ct);

        await projectionsDb.UserPRs.Where(r => r.UserId == userId).ExecuteDeleteAsync(ct);
        await projectionsDb.ExerciseProgress.Where(p => p.UserId == userId).ExecuteDeleteAsync(ct);
        await projectionsDb.WeeklyVolume.Where(w => w.UserId == userId).ExecuteDeleteAsync(ct);
        await projectionsDb.DashboardStats.Where(s => s.UserId == userId).ExecuteDeleteAsync(ct);

        var progress = new List<ExerciseProgressReadModel>();
        var weekly = new Dictionary<DateOnly, WeeklyVolumeReadModel>();
        var personalRecords = new Dictionary<Guid, PersonalRecordReadModel>();
        DashboardStatsReadModel? stats = null;

        foreach (var (date, exerciseLog) in loggedExercises)
        {
            var totalVolume = exerciseLog.Sets.Sum(s => s.Weight.Kg * s.Repetitions.Value);
            var bestSet = exerciseLog.Sets
                .OrderByDescending(s => s.Weight.Kg)
                .ThenBy(s => s.Repetitions.Value)
                .First();
            var maxWeight = bestSet.Weight.Kg;
            var bestReps = bestSet.Repetitions.Value;
            var estimated1Rm = OneRepMaxEstimator.Epley(maxWeight, bestReps);

            progress.Add(new ExerciseProgressReadModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ExerciseId = exerciseLog.ExerciseId,
                ExerciseName = exerciseLog.ExerciseName.Value,
                WorkoutDate = date,
                MaxWeightKg = maxWeight,
                TotalVolume = totalVolume,
                SetCount = exerciseLog.Sets.Count
            });

            var weekStart = GetWeekStart(date);
            if (!weekly.TryGetValue(weekStart, out var weekVolume))
            {
                weekVolume = new WeeklyVolumeReadModel
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    WeekStart = weekStart,
                    UpdatedAt = DateTime.UtcNow
                };
                weekly[weekStart] = weekVolume;
            }
            weekVolume.TotalVolume += totalVolume;
            weekVolume.SessionCount++;
            weekVolume.UpdatedAt = DateTime.UtcNow;

            if (!personalRecords.TryGetValue(exerciseLog.ExerciseId, out var record)
                || maxWeight > record.WeightKg)
            {
                personalRecords[exerciseLog.ExerciseId] = new PersonalRecordReadModel
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ExerciseId = exerciseLog.ExerciseId,
                    ExerciseName = exerciseLog.ExerciseName.Value,
                    WeightKg = maxWeight,
                    Reps = bestReps,
                    Estimated1RM = estimated1Rm,
                    AchievedAt = date
                };
            }

            stats ??= new DashboardStatsReadModel { UserId = userId };
            stats.TotalSessions++;
            stats.TotalVolumeKg += totalVolume;
            stats.LastWorkoutAt = date;
            stats.UpdatedAt = DateTime.UtcNow;
        }

        projectionsDb.UserPRs.AddRange(personalRecords.Values);
        projectionsDb.ExerciseProgress.AddRange(progress);
        projectionsDb.WeeklyVolume.AddRange(weekly.Values);
        if (stats is not null)
            projectionsDb.DashboardStats.Add(stats);

        await projectionsDb.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    private static List<DateOnly> GenerateSessionDates(int count)
    {
        var day = DateOnly.FromDateTime(DateTime.Today);
        if (day.DayOfWeek == DayOfWeek.Wednesday)
            day = day.AddDays(-1);

        var dates = new List<DateOnly>(count);
        while (dates.Count < count)
        {
            dates.Add(day);
            day = day.AddDays(-3);
            if (day.DayOfWeek == DayOfWeek.Wednesday)
                day = day.AddDays(-1);
        }

        dates.Reverse();
        return dates;
    }

    private static DateOnly GetWeekStart(DateOnly date)
    {
        var diff = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return date.AddDays(-diff);
    }
}