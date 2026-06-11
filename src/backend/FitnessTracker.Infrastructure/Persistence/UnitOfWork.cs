using FitnessTracker.Application.Common.Interfaces;

namespace FitnessTracker.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly WriteDbContext _db;

    public UnitOfWork(WriteDbContext db)
    {
        _db = db;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
}
