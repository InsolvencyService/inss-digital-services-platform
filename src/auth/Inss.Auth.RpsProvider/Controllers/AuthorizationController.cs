using Inss.Auth.RpsProvider.Application.Providers;
using Inss.Auth.RpsProvider.Domain;
using Inss.Auth.RpsProvider.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Inss.Auth.RpsProvider.Controllers;

public class AuthorizationController : Controller
{
    private readonly IUserAuthStoreProvider _userAuthStoreProvider;
    private readonly IOptions<ProviderOptions> _options;

    public AuthorizationController(IUserAuthStoreProvider userAuthStoreProvider, IOptions<ProviderOptions>  options)
    {
        _userAuthStoreProvider = userAuthStoreProvider;
        _options = options;
    }
    
    [HttpGet("/connect/authorize")]
    public async Task<IActionResult> Authorize()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            var returnUrl = Request.Path + Request.QueryString;
            return Redirect($"/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
        }

        var query = Request.Query;
        var clientId = query["client_id"].ToString();
        var redirectUri = query["redirect_uri"].ToString();
        var state = query["state"].ToString();
        var codeChallenge = query["code_challenge"].ToString();
        var codeChallengeMethod = query["code_challenge_method"].ToString();

        // TODO: Validate
        if (clientId != _options.Value.ClientId || string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(codeChallenge))
        {
            return BadRequest("Missing required parameters");
        }

        var code = Guid.NewGuid().ToString("N");
        await _userAuthStoreProvider.StoreAsync(code, new UserAuth
        {
            ClientId = clientId,
            CodeChallenge = codeChallenge,
            CodeChallengeMethod = codeChallengeMethod,
            RedirectUri = redirectUri,
            Username = User.Identity!.Name!
        });

        return Redirect($"{redirectUri}?code={code}&state={state}");
    }
}