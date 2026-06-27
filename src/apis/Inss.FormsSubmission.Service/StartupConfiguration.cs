using System.Security.Cryptography;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Inss.FormsSubmission.Service;
using Inss.FormsSubmission.Service.Endpoints;
using Inss.FormsSubmission.Service.Endpoints.Security;
using Inss.FormsSubmission.Service.Extensions;
using Inss.FormsSubmission.Service.IPUpload.Extensions;
using Inss.FormsSubmission.Service.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notify.Client;
using Notify.Interfaces;

[assembly: HostingStartup(typeof(StartupConfiguration))]

namespace Inss.FormsSubmission.Service;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            IConfigurationRoot config = configurationBuilder.Build();
            string? configFileOverride = config["config"];
            
            if (configFileOverride is not null && File.Exists(configFileOverride)){
                configurationBuilder.AddJsonFile(configFileOverride, optional: true);
            }
        });
        
        builder.ConfigureServices((context, services) =>
        {
            TokenOptions tokenOptions = new();
            context.Configuration.GetSection("Token").Bind(tokenOptions);
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = BuildTokenValidationParameters(tokenOptions);
                    options.Events.OnTokenValidated = HandleTokenValidated;
                    options.Events.OnAuthenticationFailed = HandleAuthenticationFailed;
                });
            
            services.AddAuthorizationBuilder()
                .AddSubmissionPolicy();

            services.AddIPUploadServices(context);
            services.AddOpenTelemetry().UseAzureMonitor();
            
            services.AddOptions<NotifyOptions>()
                .Bind(context.Configuration.GetSection("Notify"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddTransient<INotificationClient>(p =>
            {
                IOptions<NotifyOptions> notifyOptions = p.GetRequiredService<IOptions<NotifyOptions>>();
                return new NotificationClient(notifyOptions.Value.ApiKey);
            });
        });
        
        builder.Configure(app =>
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(configure =>
            {
                configure.DefineRootEndpoint();
                configure.DefineHealthEndpoint();
                configure.DefineSubmitIPUploadEndpoint();
            });
        });
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