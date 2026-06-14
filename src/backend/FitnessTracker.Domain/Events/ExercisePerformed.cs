using FitnessTracker.Domain.Interfaces;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Events;

public record ExercisePerformed(
    SessionId SessionId,
    UserId UserId,
    Guid ExerciseId,
    ExerciseName ExerciseName,
    DateOnly Date,
    IReadOnlyList<SetRecord> Sets
) : IDomainEvent;

public record SetRecord(decimal WeightKg, int Reps);
