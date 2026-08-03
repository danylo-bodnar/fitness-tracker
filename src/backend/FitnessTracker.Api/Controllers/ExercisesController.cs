using FitnessTracker.Api.RateLimiting;
using FitnessTracker.Application.Exercises.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Api.Controllers;

[ApiController]
[Route("exercises")]
[Authorize]
public class ExercisesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [RateLimit(RateLimitPolicy.Api)]
    public async Task<IActionResult> GetExercises(
        [FromQuery] string? muscleGroup,
        CancellationToken ct)
    {
        var exercises = await sender.Send(new GetExercisesQuery(muscleGroup), ct);
        return Ok(exercises);
    }

    [HttpGet("{id:guid}")]
    [RateLimit(RateLimitPolicy.Api)]
    public async Task<IActionResult> GetExercise(Guid id, CancellationToken ct)
    {
        try
        {
            var exercise = await sender.Send(new GetExerciseQuery(id), ct);
            return Ok(exercise);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
