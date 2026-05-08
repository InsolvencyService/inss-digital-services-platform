using Inss.Auth.Broker.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.Broker.Controllers;

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
        var openIdConnectScheme = Request.Query["login_hint"].ToString();
        _logger.SchemeLogout(openIdConnectScheme, postLogoutRedirectUri);
        return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, openIdConnectScheme);
    }
}