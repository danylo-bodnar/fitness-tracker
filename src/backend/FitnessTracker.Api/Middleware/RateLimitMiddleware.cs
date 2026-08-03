using System.Security.Claims;
using FitnessTracker.Api.RateLimiting;
using FitnessTracker.Infrastructure.RateLimiting;
using FitnessTracker.Infrastructure.RateLimiting.SlidingWindow;
using FitnessTracker.Infrastructure.RateLimiting.TokenBucket;

namespace FitnessTracker.Api.Middleware;

public sealed class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenBucketRateLimiter _tokenBucket;
    private readonly ISlidingWindowRateLimiter _slidingWindow;

    public RateLimitMiddleware(
        RequestDelegate next,
        ITokenBucketRateLimiter tokenBucket,
        ISlidingWindowRateLimiter slidingWindow)
    {
        _next = next;
        _tokenBucket = tokenBucket;
        _slidingWindow = slidingWindow;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        var policy = endpoint?
            .Metadata
            .GetMetadata<RateLimitAttribute>();

        if (policy is null)
        {
            await _next(context);
            return;
        }

        RateLimitResult result = policy.Policy switch
        {
            RateLimitPolicy.Authentication =>
                await _slidingWindow.AllowAsync(
                    $"auth:{GetIp(context)}"),

            RateLimitPolicy.Api =>
                await _tokenBucket.AllowAsync(
                    $"api:{GetUser(context)}"),

            _ => new RateLimitResult
            {
                IsAllowed = true
            }
        };

        if (!result.IsAllowed)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync(
                "Too many requests.");

            return;
        }

        await _next(context);
    }

    private static string GetIp(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
    }

    private static string GetUser(HttpContext context)
    {
        return context.User.FindFirstValue(
            ClaimTypes.NameIdentifier)
            ?? "anonymous";
    }
}