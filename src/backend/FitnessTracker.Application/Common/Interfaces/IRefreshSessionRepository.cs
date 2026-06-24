using FitnessTracker.Domain.Entities;

namespace FitnessTracker.Application.Common.Interfaces;

public interface IRefreshSessionRepository
{
    Task CreateAsync(RefreshSession session, CancellationToken ct);
    Task<RefreshSession?> GetByTokenAsync(string token, CancellationToken ct);
    Task UpdateAsync(RefreshSession session, CancellationToken ct);
    Task RevokeAllForUserAsync(Guid userId, CancellationToken ct);
}
