using FitnessTracker.Api.Middleware;

namespace FitnessTracker.Api.Extensions;

public static class RateLimitingExtensions
{
    public static IApplicationBuilder UseRateLimiting(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitMiddleware>();
    }
}
