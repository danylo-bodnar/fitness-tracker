using System.Text.Json;
using StackExchange.Redis;

namespace FitnessTracker.Api.Telegram;

public class WorkoutStateService(IConnectionMultiplexer redis)
{
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(3);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static RedisKey Key(long chatId) => new($"workout-state:{chatId}");

    public async Task<WorkoutConversationState?> GetAsync(long chatId)
    {
        var value = await redis.GetDatabase().StringGetAsync(Key(chatId));
        if (value.IsNullOrEmpty)
            return null;

        var state = JsonSerializer.Deserialize<WorkoutConversationState>((string)value!, JsonOptions);
        if (state is null || DateTime.UtcNow > state.ExpiresAt)
        {
            await DeleteAsync(chatId);
            return null;
        }

        return state;
    }

    public async Task SaveAsync(long chatId, WorkoutConversationState state)
    {
        var json = JsonSerializer.Serialize(state, JsonOptions);
        await redis.GetDatabase().StringSetAsync(Key(chatId), json, Ttl);
    }

    public async Task DeleteAsync(long chatId)
    {
        await redis.GetDatabase().KeyDeleteAsync(Key(chatId));
    }

    public async Task<bool> ExistsAsync(long chatId)
    {
        return await redis.GetDatabase().KeyExistsAsync(Key(chatId));
    }
}
