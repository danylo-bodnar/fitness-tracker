namespace FitnessTracker.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        public Task CommitAsync(CancellationToken cancellationToken);
    }
}