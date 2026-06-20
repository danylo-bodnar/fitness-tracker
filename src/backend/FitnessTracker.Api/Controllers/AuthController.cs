using System.Text;
using System.Text.Json;
using FitnessTracker.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("start-telegram-login")]
    public async Task<IActionResult> Start(CancellationToken ct)
    {
        var result = await _mediator.Send(new StartTelegramLoginCommand(), ct);
        return Ok(result);
    }

    [HttpGet("stream/{nonce}")]
    public async Task Stream(string nonce, CancellationToken ct)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        await foreach (var evt in _mediator.CreateStream(new StreamTelegramLoginQuery(nonce), ct))
        {
            await WriteEvent(Response.Body, evt.EventType, evt.Data, ct);
            await Response.Body.FlushAsync(ct);
        }
    }

    private static async Task WriteEvent(Stream body, string eventType, object data, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(data);
        var payload = $"event: {eventType}\ndata: {json}\n\n";
        await body.WriteAsync(Encoding.UTF8.GetBytes(payload), ct);
    }
}
