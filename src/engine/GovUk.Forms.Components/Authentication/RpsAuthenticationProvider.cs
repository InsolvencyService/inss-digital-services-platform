using Microsoft.AspNetCore.Http;

namespace GovUk.Forms.Components.Authentication;

public sealed class RpsAuthenticationProvider : IAuthenticationProvider
{
    public string Name => "Rps";

    public bool CanAccess(HttpContext context)
    {
        return context.User.Identity?.IsAuthenticated == true;
    }
}