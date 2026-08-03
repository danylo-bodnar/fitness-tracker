using System.Security.Claims;

namespace FitnessTracker.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("NameIdentifier claim missing");

        return Guid.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException($"Invalid userId claim: {value}");
    }
}
