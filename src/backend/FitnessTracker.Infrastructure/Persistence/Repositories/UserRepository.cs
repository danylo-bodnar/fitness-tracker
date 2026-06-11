using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public class UserRepository(WriteDbContext db) : IUserRepository
{
    private readonly WriteDbContext _db = db;

    public async Task<User?> GetByTelegramChatIdAsync(long telegramChatId, CancellationToken ct = default)
    {
        return await _db.Users
            .Where(x => x.TelegramChatId == telegramChatId)
            .SingleOrDefaultAsync(ct);
    }

    public void Add(User user)
    {
        _db.Users.Add(user);
    }
}
