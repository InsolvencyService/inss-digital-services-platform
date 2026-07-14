using Microsoft.AspNetCore.Http;

namespace GovUk.Forms.Components.Authentication;

public sealed class OneLoginAuthenticationProvider : IAuthenticationProvider
{
    public string Name => "OneLogin";
    
    public bool CanAccess(HttpContext context)
    {
        return context.User.Identity?.IsAuthenticated == true;
    }
}