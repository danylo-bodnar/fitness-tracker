using System.Text;
using System.Text.Json;
using FitnessTracker.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [HttpPost("start-telegram-login")]
    public async Task<IActionResult> Start(CancellationToken ct)
    {
        var result = await mediator.Send(new StartTelegramLoginCommand(), ct);
        return Ok(result);
    }

    [HttpGet("stream/{nonce}")]
    public async Task Stream(string nonce, CancellationToken ct)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        try
        {
            await foreach (var evt in mediator.CreateStream(new StreamTelegramLoginQuery(nonce), ct))
            {
                await WriteEvent(Response.Body, evt.EventType, evt.Data, ct);
                await Response.Body.FlushAsync(ct);
            }
        }
        catch (OperationCanceledException)
        {
            // client disconnected
        }
        catch (Exception)
        {
            try
            {
                await WriteEvent(Response.Body, "error", new { message = "Stream failed" }, ct);
                await Response.Body.FlushAsync(ct);
            }
            catch { /* connection likely dead */ }
        }
    }

    private static async Task WriteEvent(Stream body, string eventType, object data, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var payload = $"event: {eventType}\ndata: {json}\n\n";
        await body.WriteAsync(Encoding.UTF8.GetBytes(payload), ct);
    }
}