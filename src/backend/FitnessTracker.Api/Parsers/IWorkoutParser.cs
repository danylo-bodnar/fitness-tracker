using FitnessTracker.Application.Workouts.Commands;

namespace FitnessTracker.Api.Parsers;

public interface IWorkoutParser
{
    LogWorkoutCommand Parse(string text, Guid userId, DateOnly date);
}
