using System.Security.Cryptography;
using Inss.Common.IPUpload;
using Inss.FormsSubmission.Service;
using Inss.FormsSubmission.Service.Endpoints;
using Inss.FormsSubmission.Service.Endpoints.Security;
using Inss.FormsSubmission.Service.Extensions;
using Inss.FormsSubmission.Service.Handlers;
using Inss.FormsSubmission.Service.IPUpload;
using Inss.FormsSubmission.Service.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

[assembly: HostingStartup(typeof(StartupConfiguration))]

namespace Inss.FormsSubmission.Service;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
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
            
            services.AddTransient<IHandler<SubmitIPUploadRequest, SubmitIPUploadResponse>, SubmitIPUploadHandler>();
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