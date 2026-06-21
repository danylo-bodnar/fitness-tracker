using System.Net;
using System.Text.Json;
using FitnessTracker.Domain.Exceptions;

namespace FitnessTracker.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = GetStatusCode(ex);

        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(ex, "Unhandled exception occurred");
        else
            logger.LogWarning(ex, "Handled exception: {Message}", ex.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            error = ex.GetType().Name,
            message = ex.Message
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static HttpStatusCode GetStatusCode(Exception ex) => ex switch
    {
        // Auth
        LoginSessionNotFoundException => HttpStatusCode.NotFound,
        LoginSessionExpiredException => HttpStatusCode.BadRequest,
        LoginSessionAlreadyUsedException => HttpStatusCode.BadRequest,
        UserNotFoundException => HttpStatusCode.NotFound,

        // Workouts
        ExerciseNotFoundException => HttpStatusCode.NotFound,
        DuplicateExerciseException => HttpStatusCode.Conflict,

        // Programs
        ProgramDayNotFoundException => HttpStatusCode.NotFound,
        ProgramDayAlreadyExists => HttpStatusCode.Conflict,

        // Value object validation
        InvalidExerciseNameException => HttpStatusCode.BadRequest,
        InvalidRepetitionsException => HttpStatusCode.BadRequest,
        InvalidSetsException => HttpStatusCode.BadRequest,
        InvalidWeightException => HttpStatusCode.BadRequest,

        // Generic domain rule violation
        DomainException => HttpStatusCode.BadRequest,

        // Anything else is unexpected
        _ => HttpStatusCode.InternalServerError
    };
}
