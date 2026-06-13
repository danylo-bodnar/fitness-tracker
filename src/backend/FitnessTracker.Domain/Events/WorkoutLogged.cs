using FitnessTracker.Domain.Interfaces;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.Domain.Events;

public record WorkoutLogged(SessionId SessionId, UserId UserId) : IDomainEvent;
