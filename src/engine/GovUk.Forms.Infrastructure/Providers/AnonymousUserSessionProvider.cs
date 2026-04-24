using System.Text;
using GovUk.Forms.Application.Providers;
using Microsoft.AspNetCore.Http;

namespace GovUk.Forms.Infrastructure.Providers;

public sealed class AnonymousUserSessionProvider : IUserSessionProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CookieName = ".Anonymous.Form";
    private const string SessionItemId = "UserSessionId";

    public AnonymousUserSessionProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Task<string> ResolveAsync()
    {
        string? userSessionId = _httpContextAccessor.HttpContext!.Items[SessionItemId]?.ToString();

        if (string.IsNullOrWhiteSpace(userSessionId))
        {
            userSessionId = _httpContextAccessor.HttpContext?.Request.Cookies[CookieName];
            
            if (string.IsNullOrWhiteSpace(userSessionId))
            {
                userSessionId = Guid.NewGuid().ToString();
                _httpContextAccessor.HttpContext!.Items[SessionItemId] = userSessionId;
                var options = new CookieOptions
                {
                    HttpOnly = true, 
                    Secure = true, 
                    SameSite = SameSiteMode.Strict, 
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30) // This does not slide so will expire after 30mins and you might see an error
                };
                byte[] userSessionIdBytes = Encoding.UTF8.GetBytes(userSessionId);
                string encodedUserSessionId = Convert.ToBase64String(userSessionIdBytes);
                _httpContextAccessor.HttpContext?.Response.Cookies.Append(CookieName, encodedUserSessionId, options);
            }
            else
            {
                byte[] userSessionIdBytes = Convert.FromBase64String(userSessionId);
                userSessionId = Encoding.UTF8.GetString(userSessionIdBytes);
            }
        }
        
        return Task.FromResult(userSessionId);
    }
}