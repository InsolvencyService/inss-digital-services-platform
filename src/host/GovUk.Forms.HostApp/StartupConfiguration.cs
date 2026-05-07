using System.Security.Cryptography;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Components.Authentication;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Components.Options;
using GovUk.Forms.Infrastructure.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using StartupConfiguration = GovUk.Forms.HostApp.StartupConfiguration;

[assembly: HostingStartup(typeof(StartupConfiguration))]

namespace GovUk.Forms.HostApp;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            BrokerOptions brokerOptions = new();
            context.Configuration.GetSection("Broker").Bind(brokerOptions);

            services.AddSingleton<IAuthenticationProvider>(_ =>
            {
                return brokerOptions.IdentityProvider switch
                {
                    IdentityProviderTypes.Rps => new RpsAuthenticationProvider(),
                    IdentityProviderTypes.Entra => new EntraAuthenticationProvider(),
                    IdentityProviderTypes.OneLogin => new OneLoginAuthenticationProvider(),
                    _ => new AnonymousAuthenticationProvider()
                };
            });
            
            services.AddSingleton<IUserSessionProvider>(provider =>
            {
                IHttpContextAccessor accessor = provider.GetRequiredService<IHttpContextAccessor>();
                return brokerOptions.IdentityProvider switch
                {
                    IdentityProviderTypes.Rps or 
                        IdentityProviderTypes.Entra or 
                        IdentityProviderTypes.OneLogin => 
                        new AuthenticatedUserSessionProvider(accessor),
                    _ => new AnonymousUserSessionProvider(accessor)
                };
            });

            if (brokerOptions.IdentityProvider is not null)
            {
                services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    })
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                    {
                        ConfigureBaseOptions(options, brokerOptions);

                        ConfigureTokenValidation(options, brokerOptions);

                        options.GetClaimsFromUserInfoEndpoint = true;
                        
                        options.ClaimActions.MapAll();
                        
                        options.Events = new OpenIdConnectEvents
                        {
                            OnRedirectToIdentityProvider = HandleProviderRedirect,
                            OnAuthenticationFailed = HandleAuthenticationFailed,
                            OnRedirectToIdentityProviderForSignOut = HandleProviderRedirectForSignOut
                        };
                    });
            }

            services.AddSingleton<IAuthorizationHandler, DynamicAccessHandler>();
            services.AddAuthorizationBuilder()
                .AddPolicy("DynamicAccessPolicy", policy => policy.Requirements.Add(new DynamicAccessRequirement()));
            services.AddOpenTelemetry().UseAzureMonitor();
        });
    }
    
    private static void ConfigureBaseOptions(OpenIdConnectOptions options, BrokerOptions brokerOptions)
    {
        options.Authority = brokerOptions.Authority;
        options.ClientId = brokerOptions.ClientId;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SignedOutCallbackPath = "/signout-callback-oidc";
        options.SignedOutRedirectUri = "/";
        options.SaveTokens = true;
        options.Scope.Clear();

        foreach (string scope in brokerOptions.Scopes)
        {
            options.Scope.Add(scope);
        }
    }
    
    private static void ConfigureTokenValidation(OpenIdConnectOptions options, BrokerOptions brokerOptions)
    {
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
        options.ProtocolValidator.RequireNonce = false;
                    
        RSA rsa = RSA.Create();
        
        rsa.ImportFromPem(brokerOptions.JwtPublicKey);
                    
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = brokerOptions.Authority,
            ValidateAudience = true,
            ValidAudience = brokerOptions.ClientId,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
    }
    
    private static Task HandleAuthenticationFailed(AuthenticationFailedContext context)
    {
        ILogger? logger = context.HttpContext.RequestServices.GetService<ILogger>();
        logger?.AuthenticationFailed(context.Exception.Message);
        context.Response.Redirect("/error");
        return Task.CompletedTask;
    }

    private static Task HandleProviderRedirect(RedirectContext context)
    {
        IAuthenticationProvider authenticationProvider = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationProvider>();
        context.ProtocolMessage.LoginHint = authenticationProvider.Name;
        return Task.CompletedTask;
    }
    
    private static Task HandleProviderRedirectForSignOut(RedirectContext context)
    {
        IOptions<BrokerOptions> brokerOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<BrokerOptions>>();
        context.ProtocolMessage.PostLogoutRedirectUri = brokerOptions.Value.LogoutRedirectUrl;
        return Task.CompletedTask;
    }
}