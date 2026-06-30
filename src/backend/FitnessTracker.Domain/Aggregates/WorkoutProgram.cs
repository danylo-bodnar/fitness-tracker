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
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
    }

    public WorkoutProgram(Guid userId, string name, List<ProgramDay> programDays)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;

        _days.AddRange(programDays);
    }

    public ProgramDay AddDay(string name, List<ProgramExercise> exercises)
    {
        var existingDay = _days.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (existingDay != null)
        {
            throw new ProgramDayAlreadyExistsException(name);
        }

        var day = new ProgramDay(name, exercises);

        _days.Add(day);

        return day;
    }

    public void RemoveDay(Guid dayId)
    {
        var day = _days.FirstOrDefault(d => d.Id == dayId)
            ?? throw new ProgramDayNotFoundException(dayId);

        _days.Remove(day);
    }
}