using GovUk.Forms.Components.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Inss.Auth.RpsProvider.Controllers;

public class DiscoveryController : Controller
{
    private readonly IOptions<HeaderOptions> _headerOptions;
    private static readonly string[] _codeChallengeMethodsSupported = ["S256", "plain"];
    private static readonly string[] _idTokenSigningAlgValuesSupported = ["RS256"];
    private static readonly string[] _responseTypesSupported = ["code"];
    private static readonly string[] _subjectTypesSupported = ["public"];

    public DiscoveryController(IOptions<HeaderOptions> headerOptions)
    {
        _headerOptions = headerOptions;
    }
    
    [HttpGet("/.well-known/openid-configuration")]
    public IActionResult Discovery()
    {
        string issuer = _headerOptions.Value.HomeLink.Replace("/home", string.Empty);
        return Json(new
        {
            issuer,
            authorization_endpoint = $"{issuer}/connect/authorize",
            token_endpoint = $"{issuer}/connect/token",
            jwks_uri = $"{issuer}/jwks",
            response_types_supported = _responseTypesSupported,
            subject_types_supported = _subjectTypesSupported,
            end_session_endpoint = $"{issuer}/connect/endsession",
            id_token_signing_alg_values_supported = _idTokenSigningAlgValuesSupported,
            code_challenge_methods_supported = _codeChallengeMethodsSupported
        });
    }
}