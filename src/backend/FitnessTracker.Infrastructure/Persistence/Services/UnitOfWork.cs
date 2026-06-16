using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Infrastructure.Persistence.DbContexts;

namespace FitnessTracker.Infrastructure.Persistence.Services;

public class UnitOfWork(AppDbContext db, IDomainEventDispatcher dispatcher) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await dispatcher.DispatchAsync(cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
