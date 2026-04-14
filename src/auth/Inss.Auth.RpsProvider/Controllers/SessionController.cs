using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.RpsProvider.Controllers;

public class SessionController : Controller
{
    [HttpGet("/connect/endsession")]
    public IActionResult EndSession()
    {
        SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
        var postLogoutRedirectUri = Request.Query["post_logout_redirect_uri"];
        
        if (!string.IsNullOrEmpty(postLogoutRedirectUri))
        {
            return Redirect(postLogoutRedirectUri!);
        }

        return Ok("You have been logged out.");
    }
}