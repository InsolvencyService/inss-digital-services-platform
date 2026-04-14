using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.Broker.Controllers;

public class DiscoveryController : Controller
{
    private static readonly string[] _codeChallengeMethodsSupported = ["S256", "plain"];
    private static readonly string[] _idTokenSigningAlgValuesSupported = ["RS256"];
    private static readonly string[] _responseTypesSupported = ["code"];
    private static readonly string[] _subjectTypesSupported = ["public"];

    [HttpGet("/.well-known/openid-configuration")]
    public IActionResult Discovery()
    {
        var issuer = $"{Request.Scheme}://{Request.Host}";
        return Json(new
        {
            issuer,
            authorization_endpoint = $"{issuer}/connect/authorize",
            token_endpoint = $"{issuer}/connect/token",
            userinfo_endpoint = $"{issuer}/connect/userinfo",
            jwks_uri = $"{issuer}/jwks",
            response_types_supported = _responseTypesSupported,
            subject_types_supported = _subjectTypesSupported,
            end_session_endpoint = $"{issuer}/connect/endsession",
            id_token_signing_alg_values_supported = _idTokenSigningAlgValuesSupported,
            code_challenge_methods_supported = _codeChallengeMethodsSupported
        });
    }
}