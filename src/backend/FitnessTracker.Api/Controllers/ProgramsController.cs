using FitnessTracker.Api.Extensions;
using FitnessTracker.Api.RateLimiting;
using FitnessTracker.Application.WorkoutPrograms.Commands;
using FitnessTracker.Application.WorkoutPrograms.Queries;
using FitnessTracker.Contracts.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Api.Controllers;

[ApiController]
[Route("programs")]
[Authorize]
public class ProgramsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [RateLimit(RateLimitPolicy.Api)]
    public async Task<IActionResult> GetPrograms(CancellationToken ct)
    {
        var programs = await sender.Send(new GetProgramsQuery(User.GetUserId()), ct);
        return Ok(programs);
    }

    [HttpPut("{id:guid}")]
    [RateLimit(RateLimitPolicy.Api)]
    public async Task<IActionResult> UpdateProgram(Guid id,
        UpdateProgramRequest request, CancellationToken ct)
    {
        await sender.Send(
           new UpdateProgramCommand(User.GetUserId(), id, request.Name, request.ProgramDays), ct);

        return NoContent();
    }

    [HttpPost]
    [RateLimit(RateLimitPolicy.Api)]
    public async Task<IActionResult> CreateProgram(
        CreateProgramRequest request,
        CancellationToken ct)
    {
        var programId = await sender.Send(
            new CreateProgramCommand(User.GetUserId(), request.Name, request.ProgramDays), ct);
        return CreatedAtAction(nameof(GetPrograms), new { id = programId }, programId);
    }

    [HttpDelete("{id:guid}")]
    [RateLimit(RateLimitPolicy.Api)]
    public async Task<IActionResult> DeleteProgram(Guid id, CancellationToken ct)
    {
        try
        {
            await sender.Send(new DeleteProgramCommand(User.GetUserId(), id), ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

public record CreateProgramRequest(string Name, IReadOnlyList<ProgramDayDto> ProgramDays);
public record UpdateProgramRequest(string Name, IReadOnlyList<ProgramDayDto> ProgramDays);
