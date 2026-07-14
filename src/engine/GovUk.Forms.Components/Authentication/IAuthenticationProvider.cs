using Microsoft.AspNetCore.Http;

namespace GovUk.Forms.Components.Authentication;

public interface IAuthenticationProvider
{
    string Name { get; }
    
    bool CanAccess(HttpContext context);
}