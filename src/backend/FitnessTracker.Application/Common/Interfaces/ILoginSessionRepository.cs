using FitnessTracker.Domain.Entities;

namespace FitnessTracker.Application.Common.Interfaces;

public interface ILoginSessionRepository
{
    Task CreateAsync(LoginSession session, CancellationToken ct);

    Task<LoginSession?> GetByNonceAsync(string nonce, CancellationToken ct);

    Task UpdateAsync(LoginSession session, CancellationToken ct);
}
