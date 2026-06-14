using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Events;
using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Aggregates;

public class WorkoutSession(SessionId id, UserId userId, DateOnly date) : AggregateRoot
{
    private WorkoutSession() : this(default!, default!, default!) { }
    private readonly List<ExerciseLog> _exercises = [];

    public SessionId Id { get; } = id;
    public UserId UserId { get; } = userId;
    public DateOnly Date { get; } = date;

    public static WorkoutSession Create(UserId userId, DateOnly date)
    {
        var session = new WorkoutSession(new SessionId(Guid.NewGuid()), userId, date);

        return session;
    }

    public ExerciseLog AddExercise(Guid exerciseId, ExerciseName name, DateOnly today)
    {
        if (Date != today)
            throw new PastSessionModificationException();

        if (_exercises.Any(e => e.ExerciseName == name))
            throw new DuplicateExerciseException(name.Value);

        var log = new ExerciseLog(exerciseId, name);
        _exercises.Add(log);

        return log;
    }

    public void CompleteExercise(ExerciseLog log)
    {
        AddDomainEvent(new ExercisePerformed(
              Id,
              UserId,
              log.ExerciseId,
              log.ExerciseName,
              Date,
              [.. log.Sets.Select(s => new SetRecord(
                  s.Weight.Kg,
                  s.Repetitions.Value
              ))]
          ));
    }

    public ExerciseLog? FindExercise(Guid exerciseId)
        => _exercises.FirstOrDefault(e => e.Id == exerciseId);

    public IReadOnlyList<ExerciseLog> Exercises => _exercises.AsReadOnly();
}
