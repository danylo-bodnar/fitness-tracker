using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FitnessTracker.Application.Auth.Commands;
using FitnessTracker.Application.Auth.Queries;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using FitnessTracker.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IMediator mediator, IAuthCodeStore authCodeStore, ILogger<AuthController> logger, IWebHostEnvironment env) : ControllerBase
{
    private const string RefreshTokenCookie = "X-Refresh-Token";

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
                if (evt.EventType == "success" && evt.Data is TelegramLoginResultDto dto)
                {
                    var code = authCodeStore.Store(dto.RefreshToken!);
                    var payload = new { accessToken = dto.AccessToken, user = dto.User, code };
                    await WriteEvent(Response.Body, evt.EventType, payload, ct);
                }
                else
                {
                    await WriteEvent(Response.Body, evt.EventType, evt.Data, ct);
                }

                await Response.Body.FlushAsync(ct);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            logger.LogError(ex, "SSE stream failed for nonce {Nonce}", nonce);
            try
            {
                await WriteEvent(Response.Body, "error", new { message = ex.Message }, ct);
                await Response.Body.FlushAsync(ct);
            }
            catch { }
        }
    }

    [HttpPost("exchange")]
    public IActionResult Exchange(ExchangeCodeRequest request)
    {
        var refreshToken = authCodeStore.Consume(request.Code);
        if (refreshToken is null)
            return BadRequest(new { message = "Invalid or expired code." });

        SetRefreshTokenCookie(refreshToken);
        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { message = "No refresh token." });

        try
        {
            var result = await mediator.Send(new RefreshTokenCommand(refreshToken), ct);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new { accessToken = result.AccessToken });
        }
        catch (RefreshTokenReuseException)
        {
            DeleteRefreshTokenCookie();
            return Unauthorized(new { message = "Session invalidated. Please log in again." });
        }
        catch (RefreshSessionNotFoundException)
        {
            DeleteRefreshTokenCookie();
            return Unauthorized(new { message = "Invalid session." });
        }
        catch (RefreshSessionInvalidException)
        {
            DeleteRefreshTokenCookie();
            return Unauthorized(new { message = "Session expired." });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            try { await mediator.Send(new LogoutCommand(refreshToken), ct); }
            catch (RefreshSessionNotFoundException) { }
        }

        DeleteRefreshTokenCookie();
        return NoContent();
    }

    [Authorize]
    [HttpPost("logout-everywhere")]
    public async Task<IActionResult> LogoutEverywhere(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null || !Guid.TryParse(userId, out var guid))
            return Unauthorized();

        await mediator.Send(new LogoutEverywhereCommand(guid), ct);
        DeleteRefreshTokenCookie();
        return NoContent();
    }

    private bool IsProduction => env.IsProduction();

    private void SetRefreshTokenCookie(string token)
    {
        Response.Cookies.Append(RefreshTokenCookie, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = IsProduction,
            SameSite = IsProduction ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Path = "/auth"
        });
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(RefreshTokenCookie, new CookieOptions
        {
            HttpOnly = true,
            Secure = IsProduction,
            SameSite = IsProduction ? SameSiteMode.None : SameSiteMode.Lax,
            Path = "/auth"
        });
    }

    private static async Task WriteEvent(Stream body, string eventType, object data, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var payload = $"event: {eventType}\ndata: {json}\n\n";
        await body.WriteAsync(Encoding.UTF8.GetBytes(payload), ct);
    }
}

public record ExchangeCodeRequest(string Code);

