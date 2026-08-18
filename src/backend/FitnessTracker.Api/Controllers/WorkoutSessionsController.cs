using FitnessTracker.Api.Extensions;
using FitnessTracker.Api.RateLimiting;
using FitnessTracker.Application.WorkoutSessions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Api.Controllers;

[ApiController]
[Route("workouts")]
[Authorize]
public class WorkoutSessionsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [RateLimit(RateLimitPolicy.Api)]
    public async Task<IActionResult> GetWorkoutHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var history = await sender.Send(
            new GetWorkoutHistoryQuery(User.GetUserId(), page, pageSize), ct);
        return Ok(history);
    }
}
