using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GovUk.Forms.Components.Options;
using Inss.Auth.RpsProvider.Application.Providers;
using Inss.Auth.RpsProvider.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Inss.Auth.RpsProvider.Controllers;

public class TokenController : Controller
{
    private readonly IUserAuthStoreProvider _userAuthStoreProvider;
    private readonly ITokenSecurityProvider _tokenSecurityProvider;
    private readonly IOptions<HeaderOptions> _headerOptions;

    public TokenController(
        IUserAuthStoreProvider userAuthStoreProvider,
        ITokenSecurityProvider tokenSecurityProvider, 
        IOptions<HeaderOptions> headerOptions)
    {
        _userAuthStoreProvider = userAuthStoreProvider;
        _tokenSecurityProvider = tokenSecurityProvider;
        _headerOptions = headerOptions;
    }
    
    [HttpPost("/connect/token")]
    public async Task<IActionResult> Discovery()
    {
        IFormCollection form = await Request.ReadFormAsync();
        string code = form["code"].ToString();
        string clientId = form["client_id"].ToString();
        string redirectUri = form["redirect_uri"].ToString();
        string codeVerifier = form["code_verifier"].ToString();

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
        
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, userAuth.Username),
            new(JwtRegisteredClaimNames.Name, userAuth.Username),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64)
        ];

        string issuer = _headerOptions.Value.HomeLink.Replace("/home", string.Empty);
        JwtSecurityToken token = new(
            issuer: issuer,
            audience: clientId,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: _tokenSecurityProvider.GetSigningCredentials()
        );

        string? idToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Json(new { access_token = idToken, id_token = idToken, token_type = "Bearer", expires_in = 300 });
    }
}