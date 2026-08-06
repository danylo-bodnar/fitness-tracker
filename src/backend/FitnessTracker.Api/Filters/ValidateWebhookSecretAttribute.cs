using System.Security.Cryptography;
using System.Text;
using FitnessTracker.Application.Common.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace FitnessTracker.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ValidateWebhookSecretAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var options = context.HttpContext.RequestServices
            .GetRequiredService<IOptions<TelegramOptions>>();

        if (!IsSecretValid(context.HttpContext.Request, options.Value.WebhookSecret))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();
    }

    internal static bool IsSecretValid(HttpRequest request, string expectedSecret)
    {
        if (string.IsNullOrEmpty(expectedSecret) ||
            !request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var provided))
        {
            return false;
        }

        var providedBytes = Encoding.UTF8.GetBytes(provided.ToString());
        var secretBytes = Encoding.UTF8.GetBytes(expectedSecret);

        return providedBytes.Length == secretBytes.Length
            && CryptographicOperations.FixedTimeEquals(providedBytes, secretBytes);
    }
}