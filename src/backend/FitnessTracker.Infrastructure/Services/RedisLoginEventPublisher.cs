using System.Text.Json;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using StackExchange.Redis;

namespace FitnessTracker.Infrastructure.Services;

public sealed class RedisLoginEventPublisher : ILoginEventPublisher
{
    private readonly ISubscriber _subscriber;

    public RedisLoginEventPublisher(ConnectionMultiplexer multiplexer)
    {
        _subscriber = multiplexer.GetSubscriber();
    }

    public async Task PublishApprovedAsync(string nonce, string jwt, UserDto user, CancellationToken ct)
    {
        var payload = JsonSerializer.Serialize(new { nonce, jwt, user });
        var channel = new RedisChannel($"login-approved:{nonce}", RedisChannel.PatternMode.Literal);
        await _subscriber.PublishAsync(channel, payload);
    }
}
