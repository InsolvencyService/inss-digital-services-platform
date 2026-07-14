using System.Security.Cryptography;
using Inss.Auth.RpsProvider.Application.Providers;
using Microsoft.IdentityModel.Tokens;

namespace Inss.Auth.RpsProvider.Infrastructure.Providers;

public sealed class TokenSecurityProvider : ITokenSecurityProvider, IDisposable
{
    private readonly RSA _rsa;
    private readonly RsaSecurityKey _key;
    private readonly SigningCredentials _signingCredentials;
    private readonly RSAParameters _rsaParameters;
    private const string KeyId = "7a1fb599-a15b-44a7-84c3-6744ccdfead6";

    public TokenSecurityProvider()
    {
        _rsa = RSA.Create(2048);
        _key = new RsaSecurityKey(_rsa) { KeyId = KeyId };
        _signingCredentials = new  SigningCredentials(_key, SecurityAlgorithms.RsaSha256);
        _rsaParameters = _rsa.ExportParameters(false);
    }
    
    public RsaSecurityKey GetKey()
    {
        return _key;
    }

    public SigningCredentials GetSigningCredentials()
    {
        return _signingCredentials;
    }

    public RSAParameters GetRsaParameters()
    {
        return _rsaParameters;
    }

    public void Dispose()
    {
        _rsa.Dispose();
    }
}