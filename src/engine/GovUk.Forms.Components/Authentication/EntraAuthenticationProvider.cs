using Microsoft.AspNetCore.Http;

namespace GovUk.Forms.Components.Authentication;

public sealed class EntraAuthenticationProvider : IAuthenticationProvider
{
    public string Name => "Entra";
    
    public bool CanAccess(HttpContext context)
    {
        return context.User.Identity?.IsAuthenticated == true;
    }
}