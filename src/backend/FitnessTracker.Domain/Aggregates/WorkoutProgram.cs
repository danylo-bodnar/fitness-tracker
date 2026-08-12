using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Exceptions;

namespace FitnessTracker.Domain.Aggregates;

public class WorkoutProgram : AggregateRoot
{
    const int MAX_WORKOUT_DAYS = 4;

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;

    private readonly List<ProgramDay> _days = [];
    public IReadOnlyList<ProgramDay> Days => _days.AsReadOnly();

    private WorkoutProgram() { }

    public WorkoutProgram(Guid userId, string name)
        : this(userId, name, [])
    {
    }

    public WorkoutProgram(
        Guid userId,
        string name,
        List<ProgramDay> programDays)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;

        if (programDays.Count > MAX_WORKOUT_DAYS)
        {
            throw new ProgramDayLimitExceededException(MAX_WORKOUT_DAYS);
        }

        ReplaceDays(programDays);
    }

    public ProgramDay AddDay(
        string name,
        List<ProgramExercise> exercises)
    {
        var existingDay = _days.FirstOrDefault(
            d => d.Name.Equals(
                name,
                StringComparison.OrdinalIgnoreCase));

        if (existingDay is not null)
        {
            throw new ProgramDayAlreadyExistsException(name);
        }

        var day = new ProgramDay(name, _days.Count > 0 ? _days.Max(d => d.Order) + 1 : 1, exercises);

        _days.Add(day);

        return day;
    }

    public void Rename(string newName)
    {
        Name = newName;
    }

    public void ReplaceDays(List<ProgramDay> newDays)
    {
        var toRemove = _days.Where(d => !newDays.Any(n => n.Id == d.Id)).ToList();
        foreach (var day in toRemove)
            _days.Remove(day);

        foreach (var newDay in newDays)
        {
            var existing = _days.FirstOrDefault(d => d.Id == newDay.Id);
            if (existing is not null)
            {
                existing.Name = newDay.Name;
                existing.Order = newDay.Order;
                existing.ReplaceExercises(newDay.Exercises.ToList());
            }
            else
            {
                _days.Add(newDay);
            }
        }
    }

    public void RemoveDay(Guid dayId)
    {
        var day = _days.FirstOrDefault(d => d.Id == dayId)
            ?? throw new ProgramDayNotFoundException(dayId);

        _days.Remove(day);
    }
}
