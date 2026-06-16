namespace FitnessTracker.Domain.Abstractions;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> PopEvents();
}
