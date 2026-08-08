using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Events;
using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Aggregates;

public class WorkoutSession : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateOnly Date { get; private set; }

    private readonly List<ExerciseLog> _exercises = new();
    public IReadOnlyList<ExerciseLog> Exercises => _exercises;

    private WorkoutSession() { }

    private WorkoutSession(Guid id, Guid userId, DateOnly date)
    {
        Id = id;
        UserId = userId;
        Date = date;
    }

    public static WorkoutSession Create(Guid userId, DateOnly date)
        => new(Guid.NewGuid(), userId, date);

    public ExerciseLog AddExercise(Guid exerciseId, ExerciseName name, int? supersetGroupId = null)
    {
        if (_exercises.Any(e => e.ExerciseName == name && e.SupersetGroupId == supersetGroupId))
            throw new DuplicateExerciseException(name.Value);

        var log = new ExerciseLog(exerciseId, name, supersetGroupId);
        _exercises.Add(log);

        return log;
    }

    public void CompleteExercise(ExerciseLog log)
    {
        AddDomainEvent(new ExercisePerformed(
            Guid.NewGuid(),
            Id,
            UserId,
            log.ExerciseId,
            log.ExerciseName,
            Date,
            [.. log.Sets.Select(s => new SetRecord(
                s.Weight.Kg,
                s.Repetitions.Value
            ))],
            log.SupersetGroupId
        ));
    }

    public ExerciseLog? FindExercise(Guid exerciseId)
        => _exercises.FirstOrDefault(e => e.Id == exerciseId);
}