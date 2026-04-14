using System.Security.Cryptography;
using Inss.Auth.RpsProvider.Application.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Inss.Auth.RpsProvider.Controllers;

public class JwksController : Controller
{
    private readonly ITokenSecurityProvider _tokenSecurityProvider;

    public JwksController(ITokenSecurityProvider tokenSecurityProvider)
    {
        _tokenSecurityProvider = tokenSecurityProvider;
    }
    
    [HttpGet("/jwks")]
    public IActionResult Discovery()
    {
        RsaSecurityKey key = _tokenSecurityProvider.GetKey();
        RSAParameters parameters = _tokenSecurityProvider.GetRsaParameters();
        return Json(new
        {
            keys = new[]
            {
                new {
                    kty = "RSA",
                    use = "sig",
                    kid = key.KeyId,
                    e = Base64UrlEncoder.Encode(parameters.Exponent),
                    n = Base64UrlEncoder.Encode(parameters.Modulus)
                }
            }
        });
    }
}