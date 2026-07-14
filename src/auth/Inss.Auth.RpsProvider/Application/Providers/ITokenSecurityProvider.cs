using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Inss.Auth.RpsProvider.Application.Providers;

public interface ITokenSecurityProvider
{
    RsaSecurityKey GetKey();
    SigningCredentials GetSigningCredentials();
    RSAParameters GetRsaParameters();
}