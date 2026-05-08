using Inss.Auth.RpsProvider.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.RpsProvider.Controllers;

public class SessionController : Controller
{
    private readonly ILogger<SessionController> _logger;

    public SessionController(ILogger<SessionController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("/connect/endsession")]
    public async Task<IActionResult> EndSession()
    {
        var postLogoutRedirectUri = Request.Query["post_logout_redirect_uri"].ToString();
        _logger.RpsLogout(postLogoutRedirectUri);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect(postLogoutRedirectUri);
    }
}