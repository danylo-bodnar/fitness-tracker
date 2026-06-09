namespace FitnessTracker.Domain.Interfaces;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    IReadOnlyCollection<IDomainEvent> PopEvents();
    void ClearDomainEvents();
}
