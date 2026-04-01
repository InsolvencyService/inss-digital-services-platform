using Inss.Auth.Broker.Application.Providers;
using Inss.Auth.Broker.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.Broker.Controllers;

public class AuthorizationController : Controller
{
    private readonly IAuthCodeStoreProvider _authCodeStoreProvider;

    public AuthorizationController(IAuthCodeStoreProvider  authCodeStoreProvider)
    {
        _authCodeStoreProvider = authCodeStoreProvider;
    }
    
    [HttpGet("/connect/authorize")]
    public IActionResult Authorize()
    {
        // TODO: Some validation for the caller!!
        
        var issuer = $"{Request.Scheme}://{Request.Host}";
        var redirectUri = Request.Query["redirect_uri"].ToString();
        var state = Request.Query["state"].ToString();
        var loginHint = Request.Query["login_hint"].ToString();
        var codeChallenge = Request.Query["code_challenge"].ToString();
        var codeChallengeMethod = Request.Query["code_challenge_method"].ToString();
        var props = new AuthenticationProperties
        {
            RedirectUri = $"{issuer}/connect/callback",
            Items =
            {
                ["client_redirect_uri"] = redirectUri,
                ["client_state"] = state,
                ["Provider"] = loginHint,
                ["client_code_challenge"] = codeChallenge,
                ["client_code_challenge_method"] = codeChallengeMethod
            }
        };
        
        //await HttpContext.ChallengeAsync(loginHint, props);

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
        
        var authProps = result.Properties;
        var clientRedirectUri = authProps?.Items["client_redirect_uri"];
        var clientState = authProps?.Items["client_state"];
        var clientCodeChallenge = authProps?.Items["client_code_challenge"]!;
        var clientCodeChallengeMethod = authProps?.Items["client_code_challenge_method"]!;

        if (string.IsNullOrEmpty(clientRedirectUri))
        {
            return BadRequest("Missing stored client redirect URI");
        }

        var code = Guid.NewGuid().ToString("N");
        
        await _authCodeStoreProvider.StoreAsync(code, new AuthCode
        {
            Principal = result.Principal,
            CodeChallenge = clientCodeChallenge,
            CodeChallengeMethod = clientCodeChallengeMethod
        });

        // Redirect back to the client with the token
        var finalRedirect = $"{clientRedirectUri}?code={code}&state={clientState}";
        return Redirect(finalRedirect);
    }
}