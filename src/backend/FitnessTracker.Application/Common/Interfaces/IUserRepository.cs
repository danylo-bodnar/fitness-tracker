using FitnessTracker.Domain.Aggregates;

namespace FitnessTracker.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByTelegramChatIdAsync(long telegramChatId, CancellationToken ct = default);
    void Add(User user);
}
