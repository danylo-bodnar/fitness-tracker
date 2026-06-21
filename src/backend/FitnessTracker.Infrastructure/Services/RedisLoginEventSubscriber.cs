using System.Text.Json;
using System.Threading.Channels;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using StackExchange.Redis;

namespace FitnessTracker.Infrastructure.Services;

public sealed class RedisLoginEventSubscriber(IConnectionMultiplexer redis) : ILoginEventSubscriber
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<LoginApprovedPayload?> WaitForApprovalAsync(
        string nonce, TimeSpan timeout, CancellationToken ct)
    {
        var subscriber = redis.GetSubscriber();
        var channel = new RedisChannel($"login-approved:{nonce}", RedisChannel.PatternMode.Literal);
        var queue = Channel.CreateUnbounded<string>();

        await subscriber.SubscribeAsync(channel, (_, message) =>
            queue.Writer.TryWrite(message!));

        try
        {
            var timeoutTask = Task.Delay(timeout, ct);
            var messageTask = queue.Reader.ReadAsync(ct).AsTask();

            var completed = await Task.WhenAny(messageTask, timeoutTask);
            if (completed == timeoutTask)
            {
                return null;
            }

            var payload = await messageTask;
            var data = JsonSerializer.Deserialize<RedisApprovalMessage>(payload, JsonOptions)!;

            return new LoginApprovedPayload(
                data.Jwt, data.User.Id, data.User.TelegramChatId, data.User.TelegramUsername);
        }
        finally
        {
            await subscriber.UnsubscribeAsync(channel);
        }
    }

    private record RedisApprovalMessage(string Nonce, string Jwt, UserDto User);
}