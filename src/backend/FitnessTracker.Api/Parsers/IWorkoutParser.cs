using FitnessTracker.Application.WorkoutSessions.Commands;

namespace FitnessTracker.Api.Parsers;

public interface IWorkoutParser
{
    LogWorkoutCommand Parse(string text, Guid userId, DateOnly date);
}
