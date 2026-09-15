using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Events;
using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.Services;
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
        var bestSet = log.Sets
            .OrderByDescending(s => s.Weight.Kg)
            .ThenByDescending(s => s.Repetitions.Value)
            .First();

        var estimated1Rm = OneRepMaxEstimator.Epley(bestSet.Weight.Kg, bestSet.Repetitions.Value);
        var totalVolume = log.Sets.Sum(s => s.Weight.Kg * s.Repetitions.Value);

        AddDomainEvent(new ExercisePerformed(
            Guid.NewGuid(),
            Id,
            UserId,
            log.ExerciseId,
            log.ExerciseName,
            Date,
            bestSet.Weight.Kg,
            estimated1Rm,
            bestSet.Repetitions.Value,
            totalVolume,
            log.Sets.Count,
            [.. log.Sets.Select(s => new SetRecord(s.Weight.Kg, s.Repetitions.Value))],
            log.SupersetGroupId
        ));
    }

    public void Complete()
    {
        if (_exercises.Count == 0) return;

        var totalVolume = _exercises
            .SelectMany(e => e.Sets)
            .Sum(s => s.Weight.Kg * s.Repetitions.Value);

        AddDomainEvent(new WorkoutSessionCompleted(
            Guid.NewGuid(),
            Id,
            UserId,
            Date,
            totalVolume,
            _exercises.Count
        ));
    }

    public ExerciseLog? FindExercise(Guid exerciseId)
        => _exercises.FirstOrDefault(e => e.Id == exerciseId);
}