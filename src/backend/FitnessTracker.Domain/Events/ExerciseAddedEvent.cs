using FitnessTracker.Domain.Interfaces;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Events;

public record ExerciseAdded(SessionId SessionId, ExerciseName Name) : IDomainEvent;
