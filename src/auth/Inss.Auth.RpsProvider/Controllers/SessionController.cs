using Inss.Auth.RpsProvider.Extensions;
using Inss.Auth.RpsProvider.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Inss.Auth.RpsProvider.Controllers;

public class SessionController : Controller
{
    private readonly IOptions<ProviderOptions> _options;
    private readonly ILogger<SessionController> _logger;

    public SessionController(IOptions<ProviderOptions> options, ILogger<SessionController> logger)
    {
        _options = options;
        _logger = logger;
    }
    
    [HttpGet("/connect/endsession")]
    public async Task<IActionResult> EndSession()
    {
        string postLogoutRedirectUri = Request.Query["post_logout_redirect_uri"].ToString();

        // if (!_options.Value.PostLogoutRedirectAllowed(postLogoutRedirectUri))
        // {
        //     return Forbid();
        // }
        
        _logger.RpsLogout(postLogoutRedirectUri);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect(postLogoutRedirectUri);
    }
}