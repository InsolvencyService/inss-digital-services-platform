using System.Security.Claims;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;

namespace GovUk.Forms.Infrastructure.Providers;

public sealed class AuthenticatedUserSessionProvider : IUserSessionProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticatedUserSessionProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Task<(string SessionId, string Email)> ResolveAsync()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity is ClaimsIdentity { IsAuthenticated: true, Name: not null })
        {
            string email = _httpContextAccessor.HttpContext?.User.Identity.Name!;
            string sessionId = _httpContextAccessor.HttpContext?.User.FindFirst("session_id")!.Value!;
            return Task.FromResult((sessionId, email));
        }
        
        throw new UnauthenticatedUserException("No authenticated user has been provided. Check your settings.");
    }
}