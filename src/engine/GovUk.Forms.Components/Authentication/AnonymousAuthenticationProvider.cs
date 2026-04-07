using Microsoft.AspNetCore.Http;

namespace GovUk.Forms.Components.Authentication;

public sealed class AnonymousAuthenticationProvider : IAuthenticationProvider
{
    public string Name => string.Empty;
    
    public bool CanAccess(HttpContext context)
    {
        return true;
    }
}