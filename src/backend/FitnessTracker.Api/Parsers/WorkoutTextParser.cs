using System.Text.RegularExpressions;
using FitnessTracker.Application.Workouts.Commands;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Api.Parsers;

public partial class WorkoutTextParser : IWorkoutParser
{
    public LogWorkoutCommand Parse(string text, UserId userId, DateOnly date)
    {
        var match = WorkoutPattern().Match(text.Trim());

        if (!match.Success)
            throw new ParseException("Invalid format. Use: <exercise> <weight>kg; <reps>");

        var name = new ExerciseName(match.Groups[1].Value.Trim());
        var weightKg = decimal.Parse(match.Groups[2].Value);
        var reps = match.Groups[3].Value
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();

        if (reps.Count == 0)
            throw new ParseException("At least one rep count is required.");

        return new LogWorkoutCommand(userId, date, name, weightKg, reps);
    }

    [GeneratedRegex(@"^(.+?)\s+(\d+(?:\.\d+)?)kg\s*;\s*([\d,\s]+)$", RegexOptions.IgnoreCase)]
    private static partial Regex WorkoutPattern();
}
