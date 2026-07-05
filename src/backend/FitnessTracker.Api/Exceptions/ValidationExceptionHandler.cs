using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace FitnessTracker.Api.Middleware;

internal sealed class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger)
    : IExceptionHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
            return false;

        var errors = validationException.Errors
            .GroupBy(x => x.PropertyName.ToLowerInvariant())
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );

        logger.LogWarning(
            "Validation failed on {Path}",
            httpContext.Request.Path
        );

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var body = new
        {
            status = StatusCodes.Status400BadRequest,
            title = "Validation failed",
            detail = "One or more validation errors occurred",
            errors
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(body, JsonOptions), cancellationToken);

        return true;
    }
}