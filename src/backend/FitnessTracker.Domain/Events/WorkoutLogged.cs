using FitnessTracker.Domain.Interfaces;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Events;

public record WorkoutLogged(Guid SessionId, UserId UserId) : IDomainEvent;
