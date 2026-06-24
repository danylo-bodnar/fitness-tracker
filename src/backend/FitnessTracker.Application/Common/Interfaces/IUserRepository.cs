using FitnessTracker.Domain.Aggregates;

namespace FitnessTracker.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByTelegramChatIdAsync(long telegramChatId, CancellationToken ct = default);
    void Add(User user);
}
