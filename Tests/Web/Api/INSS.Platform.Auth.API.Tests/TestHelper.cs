using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace INSS.Platform.Auth.API.Tests
{
    internal static class TestHelper
    {
        internal static string GetKeyPem(string keyFileName)
        {
            string keyFilePath = Path.Combine(AppContext.BaseDirectory, keyFileName);
            if (File.Exists(keyFilePath))
            {
                return File.ReadAllText(keyFilePath);
            }

            return string.Empty;
        }

        internal static string CreateAccessTokenForTest(string keyPem, string issuer, string audience, int expirtyMinutes, Dictionary<string, object>? additionalClaims = null)
        {
            List<Claim> claims = new()
            {
                new Claim("sub", "test-user-id"),
                new Claim("email", "test.user@example.com"),
                new Claim("role", "TestRole")
            };

            if (additionalClaims != null)
            {
                foreach (KeyValuePair<string, object> kvp in additionalClaims)
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString()!));
                }
            }

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(keyPem));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirtyMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
