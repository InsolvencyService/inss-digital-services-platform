using Inss.Auth.Broker.Application.Providers;
using Inss.Auth.Broker.Domain;
using Inss.Auth.Broker.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Inss.Auth.Broker.Controllers;

public class AuthorizationController : Controller
{
    private readonly IAuthCodeStoreProvider _authCodeStoreProvider;
    private readonly IOptions<BrokerOptions> _options;

    public AuthorizationController(IAuthCodeStoreProvider  authCodeStoreProvider, IOptions<BrokerOptions>  options)
    {
        _authCodeStoreProvider = authCodeStoreProvider;
        _options = options;
    }
    
    [HttpGet("/connect/authorize")]
    public IActionResult Authorize()
    {
        // TODO: Some validation for the caller!!
        
        string issuer = $"{Request.Scheme}://{Request.Host}";
        string clientId = Request.Query["client_id"].ToString();
        string redirectUri = Request.Query["redirect_uri"].ToString();
        string state = Request.Query["state"].ToString();
        string loginHint = Request.Query["login_hint"].ToString();
        string codeChallenge = Request.Query["code_challenge"].ToString();
        string codeChallengeMethod = Request.Query["code_challenge_method"].ToString();
        string nonce = Request.Query["nonce"].ToString();
        
        if (clientId != _options.Value.ClientId || string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(codeChallenge))
        {
            return BadRequest("Missing required parameters");
        }
        
        var props = new AuthenticationProperties
        {
            RedirectUri = $"{issuer}/connect/callback",
            Items =
            {
                ["client_redirect_uri"] = redirectUri,
                ["client_state"] = state,
                ["Provider"] = loginHint,
                ["client_code_challenge"] = codeChallenge,
                ["client_code_challenge_method"] = codeChallengeMethod,
                ["client_nonce"] = nonce
            }
        };

        return Challenge(props, loginHint);
    }

    [HttpGet("/connect/callback")]
    public async Task<IActionResult> Callback()
    {
        var result = await HttpContext.AuthenticateAsync("Cookies");
        
        if (!result.Succeeded)
        {
            return Unauthorized();
        }
        
        AuthenticationProperties? authProps = result.Properties;
        string? clientRedirectUri = authProps?.Items["client_redirect_uri"];
        string? clientState = authProps?.Items["client_state"];
        string clientCodeChallenge = authProps?.Items["client_code_challenge"]!;
        string clientCodeChallengeMethod = authProps?.Items["client_code_challenge_method"]!;
        string clientNonce = authProps?.Items["client_nonce"]!;

        if (string.IsNullOrEmpty(clientRedirectUri))
        {
            return BadRequest("Missing stored client redirect URI");
        }

        AuthCode authCode = new()
        {
            Id = Guid.NewGuid().ToString("N"),
            CodeChallenge = clientCodeChallenge,
            CodeChallengeMethod = clientCodeChallengeMethod,
            Nonce = clientNonce
        };
        authCode.AddClaimsPrincipal(result.Principal);
        await _authCodeStoreProvider.StoreAsync(authCode);
        
        var finalRedirect = $"{clientRedirectUri}?code={authCode.Id}&state={clientState}";
        return Redirect(finalRedirect);
    }
}