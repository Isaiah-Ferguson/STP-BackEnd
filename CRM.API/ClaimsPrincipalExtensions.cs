using System.Security.Claims;

namespace CRM.API;

/// <summary>
/// Shared claims helpers — replaces the CurrentUserId() helper that was copy-pasted
/// into five controllers.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// The authenticated user's id from the token ("sub", or the mapped NameIdentifier).
    /// Returns <see cref="Guid.Empty"/> when absent/invalid — callers treat that as
    /// "no access" (it matches no user row).
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? user.FindFirstValue("sub");
        return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
    }
}
