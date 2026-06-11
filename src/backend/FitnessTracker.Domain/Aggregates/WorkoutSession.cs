using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Events;
using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Aggregates;

public class WorkoutSession(SessionId id, UserId userId, DateOnly date) : AggregateRoot
{
    private WorkoutSession() : this(default!, default!, default!) { }

    private readonly List<ExerciseLog> _exercises = [];

    public SessionId Id { get; private set; } = id;
    public UserId UserId { get; private set; } = userId;
    public DateOnly Date { get; private set; } = date;

    public static WorkoutSession Create(UserId userId, DateOnly date)
    {
        var session = new WorkoutSession(new SessionId(Guid.NewGuid()), userId, date);
        session.AddDomainEvent(new WorkoutLogged(session.Id, userId));
        return session;
    }

    public ExerciseLog AddExercise(ExerciseName name, DateOnly today)
    {
        if (Date != today)
            throw new PastSessionModificationException();

        if (_exercises.Any(e => e.Name == name))
            throw new DuplicateExerciseException(name.Value);

        var log = new ExerciseLog(name);
        _exercises.Add(log);
        AddDomainEvent(new ExerciseAdded(Id, name));
        return log;
    }

    public ExerciseLog? FindExercise(Guid exerciseId)
        => _exercises.FirstOrDefault(e => e.Id == exerciseId);

    public IReadOnlyList<ExerciseLog> Exercises => _exercises.AsReadOnly();
}
