using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Inss.Auth.RpsProvider.Application.Providers;
using Inss.Auth.RpsProvider.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Inss.Auth.RpsProvider.Controllers;

public class TokenController : Controller
{
    private readonly IUserAuthStoreProvider _userAuthStoreProvider;
    private readonly ITokenSecurityProvider _tokenSecurityProvider;

    public TokenController(IUserAuthStoreProvider userAuthStoreProvider, ITokenSecurityProvider tokenSecurityProvider)
    {
        _userAuthStoreProvider = userAuthStoreProvider;
        _tokenSecurityProvider = tokenSecurityProvider;
    }
    
    [HttpPost("/connect/token")]
    public async Task<IActionResult> Discovery()
    {
        var form = await Request.ReadFormAsync();
        var code = form["code"].ToString();
        var clientId = form["client_id"].ToString();
        var redirectUri = form["redirect_uri"].ToString();
        var codeVerifier = form["code_verifier"].ToString();

        UserAuth? userAuth = await _userAuthStoreProvider.GetAsync(code);
        
        if (userAuth is null)
        {
            return BadRequest("Invalid code");
        }

        if (userAuth.ClientId != clientId || userAuth.RedirectUri != redirectUri)
        {
            return BadRequest("Invalid client or redirect URI");
        }
        
        bool pkceValid = userAuth.CodeChallengeMethod switch
        {
            "S256" => userAuth.CodeChallenge == Base64UrlEncoder.Encode(SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier))),
            _ => false
        };

        if (!pkceValid)
        {
            return BadRequest("Invalid PKCE verification");
        }

        await _userAuthStoreProvider.RemoveAsync(code);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userAuth.Username),
            new Claim(JwtRegisteredClaimNames.Name, userAuth.Username),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: $"{Request.Scheme}://{Request.Host}",
            audience: clientId,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: _tokenSecurityProvider.GetSigningCredentials()
        );

        var idToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Json(new { access_token = idToken, id_token = idToken, token_type = "Bearer", expires_in = 300 });
    }
}