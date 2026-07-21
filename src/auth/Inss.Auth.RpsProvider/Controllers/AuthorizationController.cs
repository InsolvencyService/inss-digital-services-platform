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

        IQueryCollection query = Request.Query;
        string clientId = query["client_id"].ToString();
        string redirectUri = query["redirect_uri"].ToString();
        string state = query["state"].ToString();
        string codeChallenge = query["code_challenge"].ToString();
        string codeChallengeMethod = query["code_challenge_method"].ToString();
        
        if (clientId != _options.Value.ClientId || string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(codeChallenge))
        {
            return BadRequest("Missing required parameters");
        }

        string code = Guid.NewGuid().ToString("N");
        await _userAuthStoreProvider.StoreAsync(new UserAuth
        {
            Id = code,
            ClientId = clientId,
            CodeChallenge = codeChallenge,
            CodeChallengeMethod = codeChallengeMethod,
            RedirectUri = redirectUri,
            Username = User.Identity!.Name!
        });

        return Redirect($"{redirectUri}?code={code}&state={state}");
    }
}