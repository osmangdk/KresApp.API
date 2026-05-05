using System.Security.Claims;

namespace KresApp.API.Extensions;

public static class UserExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
        => Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public static string GetRole(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Role)!.Value;
}