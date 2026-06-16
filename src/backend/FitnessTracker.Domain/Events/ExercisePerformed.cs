using FitnessTracker.Domain.Abstractions;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Events;

public record ExercisePerformed(
    Guid SessionId,
    Guid UserId,
    Guid ExerciseId,
    ExerciseName ExerciseName,
    DateOnly Date,
    IReadOnlyList<SetRecord> Sets
) : IDomainEvent;

public record SetRecord(decimal WeightKg, int Reps);
