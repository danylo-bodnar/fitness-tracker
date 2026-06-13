using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Infrastructure.Persistence.DbContexts;

namespace FitnessTracker.Infrastructure.Persistence.Services;

public class UnitOfWork(WriteDbContext db) : IUnitOfWork
{
    private readonly WriteDbContext _db = db;

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
}
