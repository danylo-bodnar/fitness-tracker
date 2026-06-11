using FitnessTracker.Application.Workouts.Commands;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Api.Parsers;

public interface IWorkoutParser
{
    LogWorkoutCommand Parse(string text, UserId userId, DateOnly date);
}
