using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Inss.Auth.Broker.Extensions;
using Inss.Auth.Broker.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Inss.Auth.Broker.Controllers;

public class UserInfoController : Controller
{
    private readonly IOptions<BrokerOptions> _brokerOptions;
    private readonly ILogger<UserInfoController> _logger;

    public UserInfoController(IOptions<BrokerOptions>  brokerOptions, ILogger<UserInfoController> logger)
    {
        _brokerOptions = brokerOptions;
        _logger = logger;
    }
    
    [HttpGet("/connect/userinfo")]
    public IActionResult UserInfo()
    {
        string authHeader = Request.Headers.Authorization.ToString();
        
        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(_brokerOptions.Value.JwtPublicKey);
            
            string issuer = Request.GetForwardedHost();
            string token = authHeader["Bearer ".Length..].Trim();
            JwtSecurityTokenHandler tokenHandler = new();
            TokenValidationParameters validationParams = new()
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = _brokerOptions.Value.ClientId,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ValidateLifetime = true
            };

            try
            {
                // This is how we can add our own claims (and permissions) to the claims list that can be enforced by the host app.
                // We can integrate with the user management system to take the incoming access token which contains an identifier
                // returned as part of the sign in process with the 3rd party identity provider (Entra, OneLogin etc) and lookup the
                // user in our system, pulling extra information to enrich the claims.
                
                ClaimsPrincipal? principal = tokenHandler.ValidateToken(token, validationParams, out _);
                Claim subject = new(JwtRegisteredClaimNames.Sub, principal.FindFirst("name")!.Value);
                List<Claim> claims = [subject];
                return Json(claims.ToDictionary(c => c.Type, c => c.Value));
            }
            catch (Exception error)
            {
                _logger.UserInfoError(error.ToString());
            }
        }
        
        return Unauthorized();
    }
}