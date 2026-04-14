using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Inss.Auth.Broker.Application.Providers;
using Inss.Auth.Broker.Domain;
using Inss.Auth.Broker.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Inss.Auth.Broker.Controllers;

public class TokenController : Controller
{
    private readonly IAuthCodeStoreProvider _authCodeStoreProvider;
    private readonly ITokenSecurityProvider _tokenSecurityProvider;
    private readonly IOptions<BrokerOptions> _brokerOptions;

    public TokenController(
        IAuthCodeStoreProvider authCodeStoreProvider, 
        ITokenSecurityProvider tokenSecurityProvider, IOptions<BrokerOptions> brokerOptions)
    {
        _authCodeStoreProvider = authCodeStoreProvider;
        _tokenSecurityProvider = tokenSecurityProvider;
        _brokerOptions = brokerOptions;
    }
    
    [HttpPost("/connect/token")]
    public async Task<IActionResult> Discovery()
    {
        var form = await Request.ReadFormAsync();
        var code = form["code"].ToString();
        var codeVerifier = form["code_verifier"].ToString();

        AuthCode? authCode = await _authCodeStoreProvider.GetAsync(code);
        
        if (authCode is null)
        {
            return BadRequest("Invalid code");
        }

        await _authCodeStoreProvider.RemoveAsync(code);

        // PKCE validation
        bool pkceValid = authCode.CodeChallengeMethod switch
        {
            "S256" => authCode.CodeChallenge == Base64UrlEncoder.Encode(SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier))),
            _ => false
        };

        if (!pkceValid)
        {
            return BadRequest("Invalid PKCE verification");
        }

        ClaimsIdentity ci = (ClaimsIdentity)authCode.Principal.Identity!;
        AppendSubjectClaim(ci);
        AppendNameClaim(ci);
    
        var tokenHandler = new JwtSecurityTokenHandler();

        SigningCredentials signingCredentials = _tokenSecurityProvider.GetSigningCredentials();
        
        var issuer = $"{Request.Scheme}://{Request.Host}";
        
        var idToken = tokenHandler.CreateJwtSecurityToken(
            issuer: issuer,
            audience: "123",
            subject: new ClaimsIdentity(authCode.Principal.Claims),
            expires: DateTime.UtcNow.AddMinutes(_brokerOptions.Value.TokenExpiresInMinutes),
            signingCredentials: signingCredentials
        );
        
        var accessToken = tokenHandler.CreateJwtSecurityToken(
            issuer: issuer,
            audience: "api",
            subject: new ClaimsIdentity(authCode.Principal.Claims),
            expires: DateTime.UtcNow.AddMinutes(_brokerOptions.Value.TokenExpiresInMinutes),
            signingCredentials: signingCredentials
        );

        return Json(new
        {
            access_token = tokenHandler.WriteToken(accessToken),
            id_token = tokenHandler.WriteToken(idToken),
            token_type = "Bearer",
            expires_in = 1800 // TODO: Config
        });
    }
    
    private static void AppendSubjectClaim(ClaimsIdentity ci)
    {
        if (!ci.HasClaim(c => c.Type == JwtRegisteredClaimNames.Sub))
        {
            Claim cl = ci.FindFirst("name") ?? ci.FindFirst("email") ?? throw new InvalidOperationException("Missing name or email claim.");
            ci.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, cl.Value));
        }
    }

    private static void AppendNameClaim(ClaimsIdentity ci)
    {
        if (!ci.HasClaim(c => c.Type == ClaimTypes.Name))
        {
            Claim cl = ci.FindFirst("name") ?? ci.FindFirst("email") ?? throw new InvalidOperationException("Missing name or email claim.");
            ci.AddClaim(new Claim(ClaimTypes.Name, cl.Value));
        }
    }
}