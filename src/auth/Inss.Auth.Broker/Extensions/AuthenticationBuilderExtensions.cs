using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Inss.Auth.Broker.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Inss.Auth.Broker.Extensions;

public static class AuthenticationBuilderExtensions
{
    extension(AuthenticationBuilder builder)
    {
        public AuthenticationBuilder AddRps()
        {
            IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
            IOptions<RpsIdentityProviderOptions> identityProviderOptions = serviceProvider.GetRequiredService<IOptions<RpsIdentityProviderOptions>>();
            
            return builder.AddOpenIdConnect("Rps", options =>
            {
                options.Authority = identityProviderOptions.Value.Authority;
                options.ClientId = identityProviderOptions.Value.ClientId;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.ResponseMode = OpenIdConnectResponseMode.Query;
                options.SaveTokens = true;
                options.Scope.Clear();

                foreach (string scope in identityProviderOptions.Value.Scopes)
                {
                    options.Scope.Add(scope);
                }

                options.ProtocolValidator.RequireNonce = false;
                options.CallbackPath = "/signin-oidc-rps";
            });
        }
        
        public AuthenticationBuilder AddOneLogin()
        {
            IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
            IOptions<OneLoginIdentityProviderOptions> identityProviderOptions = serviceProvider.GetRequiredService<IOptions<OneLoginIdentityProviderOptions>>();
            
            return builder.AddOpenIdConnect("OneLogin", options =>
            {
                options.Authority = identityProviderOptions.Value.Authority;
                options.ClientId = identityProviderOptions.Value.ClientId;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.ResponseMode = OpenIdConnectResponseMode.Query;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;
                options.Scope.Clear();

                foreach (string scope in identityProviderOptions.Value.Scopes)
                {
                    options.Scope.Add(scope);
                }
                
                options.CallbackPath = "/signin-oidc-one-login";
                
                options.Events.OnRedirectToIdentityProvider = static context =>
                {
                    context.ProtocolMessage.ResponseMode = "query";
                    context.ProtocolMessage.SetParameter("ui_locales", "en");
                    context.ProtocolMessage.SetParameter("vtr", "[\"Cl.Cm\"]");
                    return Task.CompletedTask;
                };

                options.Events.OnAuthorizationCodeReceived += context =>
                {
                    const int jwtExpiryMinutes = 5;
                    string clientId = identityProviderOptions.Value.ClientId;
                    string audienceEndpoint = $"{identityProviderOptions.Value.Authority}/token";

                    Dictionary<string, object> clientAssertionClaims = BuildClientAssertionClaims(clientId, audienceEndpoint);
                    string key = File.ReadAllText(identityProviderOptions.Value.JwtPrivateKey);
                    string clientAssertionToken = CreateJwtSecurityToken(clientId, audienceEndpoint, key, jwtExpiryMinutes, clientAssertionClaims);

                    if (context.TokenEndpointRequest is not null)
                    {
                        context.TokenEndpointRequest.ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                        context.TokenEndpointRequest.ClientAssertion = clientAssertionToken;
                    }
            
                    return Task.CompletedTask;
                };
            });
        }
        
        public AuthenticationBuilder AddEntra()
        {
            IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
            IOptions<EntraIdentityProviderOptions> identityProviderOptions = serviceProvider.GetRequiredService<IOptions<EntraIdentityProviderOptions>>();
            
            return builder.AddOpenIdConnect("Entra", options =>
            {
                options.Authority = identityProviderOptions.Value.Authority;
                options.ClientId = identityProviderOptions.Value.ClientId;
                options.ClientSecret =  identityProviderOptions.Value.ClientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.ResponseMode = OpenIdConnectResponseMode.Query;
                options.SaveTokens = true;
                options.Scope.Clear();

                foreach (string scope in identityProviderOptions.Value.Scopes)
                {
                    options.Scope.Add(scope);
                }
                
                options.CallbackPath = "/signin-oidc-entra";
            });
        }
    }
    
    // OneLogin...
    
    static Dictionary<string, object> BuildClientAssertionClaims(string clientId, string audienceEndpoint)
    {
        return new Dictionary<string, object>
        {
            { "aud", audienceEndpoint },
            { "iss", clientId },
            { "sub", clientId },
            { "jti", Guid.NewGuid().ToString() },
            { "iat", EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture) },
            { "exp", EpochTime.GetIntDate(DateTime.UtcNow.AddMinutes(5)).ToString(CultureInfo.InvariantCulture) },
        };
    }

    static string CreateJwtSecurityToken(string issuer, string audienceEndpoint, string privateKeyPem, int expiryMinutes, Dictionary<string, object> claims)
    {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);

        RsaSecurityKey privateKey = new(rsa);
        SigningCredentials credentials = new(privateKey, SecurityAlgorithms.RsaSha256);

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken token = handler.CreateJwtSecurityToken(
            issuer: issuer,
            audience: audienceEndpoint,
            subject: null,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            issuedAt: DateTime.UtcNow,
            signingCredentials: credentials,
            encryptingCredentials: null,
            claimCollection: claims
        );

        return handler.WriteToken(token);
    }
}