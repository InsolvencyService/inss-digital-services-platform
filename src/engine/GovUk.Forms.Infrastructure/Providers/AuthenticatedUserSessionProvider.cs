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
    
    public Task<string> ResolveAsync()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity is ClaimsIdentity { IsAuthenticated: true, Name: not null })
        {
            return Task.FromResult(_httpContextAccessor.HttpContext?.User.Identity.Name!);
        }
        
        throw new UnauthenticatedUserException("No authenticated user has been provided. Check your settings.");
    }
}