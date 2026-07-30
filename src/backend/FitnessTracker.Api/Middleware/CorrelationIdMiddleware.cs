using Serilog.Context;

namespace FitnessTracker.Api.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        var correlationId =
            context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? context.TraceIdentifier;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            await _next(context);
        }
    }
}
