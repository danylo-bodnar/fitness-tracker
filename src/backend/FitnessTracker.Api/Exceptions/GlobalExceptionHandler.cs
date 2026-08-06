using System.Text.Json;
using FitnessTracker.Application.Common.Exceptions;
using FitnessTracker.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace FitnessTracker.Api.Exceptions;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
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
        var statusCode = MapStatusCode(exception);
        var isServerError = statusCode == StatusCodes.Status500InternalServerError;

        if (isServerError)
        {
            logger.LogError(exception, "Unhandled exception on {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning("Handled exception on {Method} {Path}: {Message}",
                httpContext.Request.Method, httpContext.Request.Path, exception.Message);
        }

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;

        var body = new
        {
            status = statusCode,
            title = "Request failed",
            type = isServerError ? "InternalServerError" : exception.GetType().Name,
            detail = isServerError
                ? "An unexpected error occurred."
                : exception.Message
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(body, JsonOptions), cancellationToken);

        return true;
    }

    private static int MapStatusCode(Exception ex) => ex switch
    {
        // Auth
        LoginSessionNotFoundException => StatusCodes.Status404NotFound,
        LoginSessionExpiredException => StatusCodes.Status400BadRequest,
        LoginSessionAlreadyUsedException => StatusCodes.Status400BadRequest,
        UserNotFoundException => StatusCodes.Status404NotFound,

        // Workouts
        ExerciseNotFoundException => StatusCodes.Status404NotFound,
        DuplicateExerciseException => StatusCodes.Status409Conflict,

        // Programs
        ProgramDayNotFoundException => StatusCodes.Status404NotFound,
        ProgramDayAlreadyExistsException => StatusCodes.Status409Conflict,
        ProgramDayLimitExceededException => StatusCodes.Status409Conflict,
        ProgramNameEmptyException => StatusCodes.Status400BadRequest,
        ProgramNameTooLongException => StatusCodes.Status400BadRequest,
        WorkoutProgramLimitReachedException => StatusCodes.Status409Conflict,

        // Value objects
        InvalidExerciseNameException => StatusCodes.Status400BadRequest,
        InvalidRepetitionsException => StatusCodes.Status400BadRequest,
        InvalidSetsException => StatusCodes.Status400BadRequest,
        InvalidWeightException => StatusCodes.Status400BadRequest,

        // Application layer
        NotFoundException => StatusCodes.Status404NotFound,
        ForbiddenException => StatusCodes.Status403Forbidden,
        ValidationException => StatusCodes.Status400BadRequest,

        // Domain
        DomainException => StatusCodes.Status400BadRequest,

        _ => StatusCodes.Status500InternalServerError
    };
}

