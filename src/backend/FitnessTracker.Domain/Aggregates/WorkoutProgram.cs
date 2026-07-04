using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Exceptions;

namespace FitnessTracker.Domain.Aggregates;

public class WorkoutProgram : AggregateRoot
{
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

        var day = new ProgramDay(name, _days.Count + 1, exercises);

        _days.Add(day);

        return day;
    }

    public void Rename(string newName)
    {
        Name = newName;
    }

    public void ReplaceDays(List<ProgramDay> newDays)
    {
        _days.Clear();
        _days.AddRange(newDays);
    }

    public void RemoveDay(Guid dayId)
    {
        var day = _days.FirstOrDefault(d => d.Id == dayId)
            ?? throw new ProgramDayNotFoundException(dayId);

        _days.Remove(day);
    }
}
