using FitnessTracker.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace FitnessTracker.Api.Controllers;

[ApiController]
[Route("bot")]
public class BotController(
    ITelegramBotClient bot,
    IUpdateHandler updateHandler) : ControllerBase
{
    [HttpPost]
    [ValidateWebhookSecret]
    public async Task<IActionResult> Post(Update update, CancellationToken ct)
    {
        await updateHandler.HandleUpdateAsync(bot, update, ct);
        return Ok();
    }
}