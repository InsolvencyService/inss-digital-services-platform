using System.Security.Cryptography;
using Inss.Auth.Broker.Application.Providers;
using Inss.Auth.Broker.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Inss.Auth.Broker.Infrastructure.Providers;

public sealed class TokenSecurityProvider : ITokenSecurityProvider, IDisposable
{
    private readonly RSA _rsa;
    private readonly RsaSecurityKey _key;
    private readonly SigningCredentials _signingCredentials;
    private readonly RSAParameters _rsaParameters;

    public TokenSecurityProvider(IOptions<BrokerOptions> options)
    {
        _rsa = RSA.Create();
        _rsa.ImportFromPem(options.Value.JwtPrivateKey);
        _key = new RsaSecurityKey(_rsa);
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