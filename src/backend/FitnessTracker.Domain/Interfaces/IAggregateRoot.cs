namespace FitnessTracker.Domain.Interfaces;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> PopEvents();
}
