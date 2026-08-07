using System.Security.Cryptography;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Inss.Platform.Submission.Service.Endpoints.Security;
using Inss.Platform.Submission.Service.IPUpload.Extensions;
using Inss.Platform.Submission.Service.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notify.Client;
using Notify.Interfaces;

namespace Inss.Platform.Submission.Service.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAppServices(IConfiguration configuration, IWebHostEnvironment environment)
        {
            TokenOptions tokenOptions = new();
            configuration.GetSection("Token").Bind(tokenOptions);
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = BuildTokenValidationParameters(tokenOptions);
                    options.Events.OnTokenValidated = HandleTokenValidated;
                    options.Events.OnAuthenticationFailed = HandleAuthenticationFailed;
                });
            
            services.AddAuthorizationBuilder()
                .AddSubmissionPolicy();

            services.AddIPUploadServices(configuration, environment);
            services.AddOpenTelemetry().UseAzureMonitor();
            
            services.AddOptions<NotifyOptions>()
                .Bind(configuration.GetSection("Notify"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddTransient<INotificationClient>(p =>
            {
                IOptions<NotifyOptions> notifyOptions = p.GetRequiredService<IOptions<NotifyOptions>>();
                return new NotificationClient(notifyOptions.Value.ApiKey);
            });
            
            return services;
        }
    }
    
    private static TokenValidationParameters BuildTokenValidationParameters(TokenOptions tokenOptions)
    {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(tokenOptions.JwtPrivateKey);
        
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = tokenOptions.ClientId,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa) 
        };
    }
    
    private static Task HandleTokenValidated(TokenValidatedContext context)
    {
        ILogger logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.TokenValidated();
        return Task.CompletedTask;
    }
    
    private static Task HandleAuthenticationFailed(AuthenticationFailedContext context)
    {
        ILogger logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.TokenValidationFailed(context.Exception.ToString());
        return Task.CompletedTask;
    }
}