using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ECommerce.Domain.Interfaces;

namespace ECommerce.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetCustomerId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(AppClaimTypes.CustomerId)
            ?? throw new InvalidOperationException("Token has no customer_id claim.");
        return Guid.Parse(value);
    }

    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new InvalidOperationException("Token has no sub claim.");
        return Guid.Parse(value);
    }
}
