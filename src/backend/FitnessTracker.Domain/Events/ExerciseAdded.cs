using FitnessTracker.Domain.Interfaces;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Events;

public record ExerciseAdded(Guid SessionId, ExerciseName Name) : IDomainEvent;
