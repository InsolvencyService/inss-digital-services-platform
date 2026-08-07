using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Inss.Platform.Broker.Application.Providers;

public interface ITokenSecurityProvider
{
    RsaSecurityKey GetKey();
    SigningCredentials GetSigningCredentials();
    RSAParameters GetRsaParameters();
}