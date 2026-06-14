namespace FitnessTracker.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(CancellationToken cancellationToken = default);
}
