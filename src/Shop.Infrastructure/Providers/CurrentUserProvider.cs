using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shop.Core.Interfaces;

namespace Shop.Infrastructure.Providers;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAcessor;

    public CurrentUserProvider(IHttpContextAccessor httpContextAcessor)
        => _httpContextAcessor = httpContextAcessor;

    public Guid? GetCurrentUserId()
    {
        if (_httpContextAcessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var claimValue = _httpContextAcessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return claimValue != null && Guid.TryParse(claimValue, out var userId) ? userId : default;
        }

        return default;
    }
}