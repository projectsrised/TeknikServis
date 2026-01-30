using System.Security.Claims;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Enums;
using TeknikServisApp.Infrastructure.Repositories;

namespace TeknikServisApp.API.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var provider = (TenantProvider)tenantProvider;

            var tenantIdClaim = context.User.FindFirst("TenantId")?.Value;
            if (Guid.TryParse(tenantIdClaim, out var tenantId))
                provider.TenantId = tenantId;

            var bayiIdClaim = context.User.FindFirst("BayiId")?.Value;
            if (Guid.TryParse(bayiIdClaim, out var bayiId))
                provider.BayiId = bayiId;

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
                provider.KullaniciId = userId;

            var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(roleClaim) && Enum.TryParse<UserRole>(roleClaim, out var role))
                provider.Rol = role;
        }

        await _next(context);
    }
}

public static class JwtMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtMiddleware>();
    }
}
