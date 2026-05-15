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
        ITokenSecurityProvider tokenSecurityProvider, 
        IOptions<BrokerOptions> brokerOptions)
    {
        _authCodeStoreProvider = authCodeStoreProvider;
        _tokenSecurityProvider = tokenSecurityProvider;
        _brokerOptions = brokerOptions;
    }
    
    [HttpPost("/connect/token")]
    public async Task<IActionResult> TokenExchange()
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

        ClaimsPrincipal principal = authCode.GetClaimsPrincipal();
        ClaimsIdentity identity = (ClaimsIdentity)principal.Identity!;
        AppendSubjectClaim(identity);
        AppendNameClaim(identity);
        AppendNonceClaim(identity, authCode.Nonce);
    
        var tokenHandler = new JwtSecurityTokenHandler();

        SigningCredentials signingCredentials = _tokenSecurityProvider.GetSigningCredentials();
        
        var issuer = $"{Request.Scheme}://{Request.Host}";
        
        var idToken = tokenHandler.CreateJwtSecurityToken(
            issuer: issuer,
            audience: _brokerOptions.Value.ClientId,
            subject: new ClaimsIdentity(principal.Claims),
            expires: DateTime.UtcNow.AddMinutes(_brokerOptions.Value.TokenExpiresInMinutes),
            signingCredentials: signingCredentials
        );
        
        var accessToken = tokenHandler.CreateJwtSecurityToken(
            issuer: issuer,
            audience: _brokerOptions.Value.ClientId,
            subject: new ClaimsIdentity(identity.Claims),
            expires: DateTime.UtcNow.AddMinutes(_brokerOptions.Value.TokenExpiresInMinutes),
            signingCredentials: signingCredentials
        );

        return Json(new
        {
            access_token = tokenHandler.WriteToken(accessToken),
            id_token = tokenHandler.WriteToken(idToken),
            token_type = "Bearer",
            expires_in = DateTime.UtcNow.AddMinutes(_brokerOptions.Value.TokenExpiresInMinutes),
        });
    }
    
    private static void AppendSubjectClaim(ClaimsIdentity identity)
    {
        if (!identity.HasClaim(c => c.Type == JwtRegisteredClaimNames.Sub))
        {
            Claim claim = identity.FindFirst("name") 
                          ?? identity.FindFirst("email") 
                          ?? throw new InvalidOperationException("Missing name or email claim.");
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, claim.Value));
        }
    }

    private static void AppendNameClaim(ClaimsIdentity identity)
    {
        if (!identity.HasClaim(c => c.Type == ClaimTypes.Name))
        {
            Claim claim = identity.FindFirst("name") 
                          ?? identity.FindFirst("email")
                          ?? throw new InvalidOperationException("Missing name or email claim.");
            identity.AddClaim(new Claim(ClaimTypes.Name, claim.Value));
        }
    }
    
    private static void AppendNonceClaim(ClaimsIdentity identity, string nonce)
    {
        if (!identity.HasClaim(c => c.Type == Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Nonce))
        {
            identity.AddClaim(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Nonce, nonce));
        }
    }
}