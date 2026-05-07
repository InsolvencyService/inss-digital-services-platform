using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.Broker.Controllers;

public class SessionController : Controller
{
    [HttpGet("/connect/endsession")]
    public async Task<IActionResult> EndSession()
    {
        var postLogoutRedirectUri = Request.Query["post_logout_redirect_uri"];
        var openIdConnectScheme = Request.Query["login_hint"].ToString();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(openIdConnectScheme);
        
        if (!string.IsNullOrEmpty(postLogoutRedirectUri))
        {
            return Redirect(postLogoutRedirectUri!);
        }
        
        return Ok("You have been logged out.");
    }
}