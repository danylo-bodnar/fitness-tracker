using FitnessTracker.Application.Common.Interfaces;

namespace FitnessTracker.Infrastructure.Persistence;

public class UnitOfWork(WriteDbContext db) : IUnitOfWork
{
    private readonly WriteDbContext _db = db;

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
}
