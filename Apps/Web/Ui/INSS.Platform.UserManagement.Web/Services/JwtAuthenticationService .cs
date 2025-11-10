using INSS.Platform.UserManagement.Web.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace INSS.Platform.UserManagement.Web.Services
{
    public class JwtAuthenticationService : IJwtAuthenticationService
    {
        private readonly ILogger<JwtAuthenticationService> _logger;
        private readonly AuthOptions _authOptions;

        public JwtAuthenticationService(ILogger<JwtAuthenticationService> logger, IOptions<AuthOptions> authenticationOptions)
        {
            _logger = logger;
            _authOptions = authenticationOptions.Value;
        }

        public bool ValidateJwt(string jwt, out ClaimsPrincipal principal)
        {
            _logger.LogInformation("Validating JWT token.");

            principal = new();
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(_authOptions.JwtPublicKey);

            JwtSecurityTokenHandler tokenHandler = new();
            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = _authOptions.JwtIssuer,
                ValidateAudience = true,
                ValidAudience = _authOptions.JwtAudience,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsa)
            };

            try
            {
                principal = tokenHandler.ValidateToken(jwt, validationParameters, out _);
                return true;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning(ex, "JWT token has expired.");
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                _logger.LogWarning(ex, "JWT token signature is invalid.");
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "JWT token validation failed: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during JWT validation.");
            }

            return false;
        }
    }
}
