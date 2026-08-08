using FitnessTracker.Domain.Abstractions;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Events;

public record ExercisePerformed(
    Guid EventId,
    Guid SessionId,
    Guid UserId,
    Guid ExerciseId,
    ExerciseName ExerciseName,
    DateOnly Date,
    IReadOnlyList<SetRecord> Sets,
    int? SupersetGroupId = null
) : IDomainEvent;

public record SetRecord(decimal WeightKg, int Reps);
