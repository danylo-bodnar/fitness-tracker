using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    private readonly AppDbContext _db = db;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Users.FindAsync([id], ct);
    }

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
