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
    decimal MaxWeightKg,
    decimal Estimated1Rm,
    int BestSetReps,
    decimal TotalVolume,
    int SetCount,
    IReadOnlyList<SetRecord> Sets,
    int? SupersetGroupId = null
) : IDomainEvent;

public record SetRecord(decimal WeightKg, int Reps);
