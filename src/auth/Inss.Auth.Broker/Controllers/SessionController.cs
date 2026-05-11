using Inss.Auth.Broker.Extensions;
using Inss.Auth.Broker.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Inss.Auth.Broker.Controllers;

public class SessionController : Controller
{
    private readonly IOptions<BrokerOptions> _options;
    private readonly ILogger<SessionController> _logger;

    public SessionController(IOptions<BrokerOptions> options, ILogger<SessionController> logger)
    {
        _options = options;
        _logger = logger;
    }
    
    [HttpGet("/connect/endsession")]
    public IActionResult EndSession()
    {
        string postLogoutRedirectUri = Request.Query["post_logout_redirect_uri"].ToString();
        
        if (!_options.Value.PostLogoutRedirectAllowed(postLogoutRedirectUri))
        {
            _logger.InvalidPostRedirectLogoutUrl(postLogoutRedirectUri);
            string list = string.Join(Environment.NewLine, _options.Value.AllowedPostLogoutRedirects);
            throw new InvalidOperationException($"The post logout redirect {postLogoutRedirectUri} has not been matched with allowed list. Options are: {list}");
        }
        
        var openIdConnectScheme = Request.Query["login_hint"].ToString();
        _logger.SchemeLogout(openIdConnectScheme, postLogoutRedirectUri);
        return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, openIdConnectScheme);
    }
}