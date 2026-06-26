using FitnessTracker.API.Extensions;
using FitnessTracker.Application.Stats.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.API.Controllers;

[ApiController]
[Route("stats")]
[Authorize]
public class StatsController(ISender sender) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken ct)
    {
        var stats = await sender.Send(new GetDashboardQuery(User.GetUserId()), ct);
        return Ok(stats);
    }

    [HttpGet("personal-records")]
    public async Task<IActionResult> GetPersonalRecords(
        [FromQuery] Guid? exerciseId,
        CancellationToken ct)
    {
        var records = await sender.Send(
            new GetPersonalRecordsQuery(User.GetUserId(), exerciseId), ct);
        return Ok(records);
    }

    [HttpGet("exercise-progress")]
    public async Task<IActionResult> GetExerciseProgress(
        [FromQuery] Guid exerciseId,
        CancellationToken ct)
    {
        var progress = await sender.Send(
            new GetExerciseProgressQuery(User.GetUserId(), exerciseId), ct);
        return Ok(progress);
    }

    [HttpGet("weekly-volume")]
    public async Task<IActionResult> GetWeeklyVolume(
        [FromQuery] int weeks = 12,
        CancellationToken ct = default)
    {
        if (weeks is < 1 or > 52)
            return BadRequest("Weeks must be between 1 and 52.");

        var volume = await sender.Send(
            new GetWeeklyVolumeQuery(User.GetUserId(), weeks), ct);
        return Ok(volume);
    }
}
