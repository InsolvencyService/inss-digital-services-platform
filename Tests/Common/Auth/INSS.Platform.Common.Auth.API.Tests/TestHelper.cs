using INSS.Platform.Common.Auth.Contracts.Request;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace INSS.Platform.Common.Auth.API.Tests
{
    internal static class TestHelper
    {
        internal static LoginRequest CreateLoginRequest(string clientUrl = "https://client", string csrfToken = "csrf", string userId = "user")
        {
            return new ()
            {
                ClientUrl = clientUrl,
                CsrfToken = csrfToken,
                UserId = userId
            };
        }

        internal static LogoutRequest CreateLogoutRequest() 
        {
            return new() { IdToken = "idtoken" };
        }


        internal static string CreateIdToken(
            string privateKeyFileName,
            string nonce = "", 
            string csrfToken = "", 
            string userId = "", 
            string clientUrl = "", 
            string issuer = "test-issuer", 
            string audience = "https://auth")
        {
            string privateKeyPem = GetKeyPem(privateKeyFileName);
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);

            RsaSecurityKey privateKey = new(rsa);
            SigningCredentials credentials = new(privateKey, SecurityAlgorithms.RsaSha256);

            List<Claim> claims =
            [
                new Claim("nonce", nonce),
                new Claim("csrfToken", csrfToken),
                new Claim("userId", userId),
                new Claim("clientUrl", clientUrl),
                new Claim(JwtRegisteredClaimNames.Sub, "user-123"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.CurrentCulture), ClaimValueTypes.Integer64),
                new Claim("scope", "api.read api.write"),
                new Claim("roles", "admin")
            ];

            JwtSecurityToken token = new (
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        internal static string CreateAccessToken(
            string privateKeyFileName,
            string issuer = "test-issuer", 
            string audience = "test-audience")
        {
            string privateKeyPem = GetKeyPem(privateKeyFileName);
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);

            RsaSecurityKey privateKey = new(rsa);
            SigningCredentials credentials = new(privateKey, SecurityAlgorithms.RsaSha256);

            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, "user-123"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.CurrentCulture), ClaimValueTypes.Integer64)
            ];

            JwtSecurityToken token = new(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GetKeyPem(string keyFileName)
        {
            string keyFilePath = Path.Combine(AppContext.BaseDirectory, keyFileName);
            if (File.Exists(keyFilePath))
            {
                return File.ReadAllText(keyFilePath);
            }

            return string.Empty;
        }
    }
}
