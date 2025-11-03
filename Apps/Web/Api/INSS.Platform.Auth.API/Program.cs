using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.API.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace INSS.Platform.Auth.API
{
    /// <summary>
    /// Entry point for the Auth API service.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "There is no business logic in this class to test.")]
    public static class Program
    {
        /// <summary>
        /// Configures and runs the Auth API web service.
        /// </summary>
        /// <param name="args">Command-line arguments for the application.</param>
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Logging.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "[HH:mm:ss] ";
                options.IncludeScopes = true;
            });


            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddApplicationInsightsTelemetry();

            // Register AuthProviderOptions for DI with controllers and services.
            ConfigurationManager configuration = builder.Configuration;
            builder.Services.AddOptions<AuthProviderOptions>()
                .Bind(builder.Configuration.GetSection("AuthProviderOptions"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddSingleton(sp =>
            {
                string? keyVaultUriString = configuration["KeyVault:Uri"] ?? string.Empty;
                if (string.IsNullOrWhiteSpace(keyVaultUriString))
                {
                    throw new InvalidOperationException("KeyVault URI is missing.");
                }

                return new SecretClient(new Uri(keyVaultUriString), new DefaultAzureCredential());
            });

            builder.Services.AddScoped<OneLoginAuthService>();
            builder.Services.AddScoped<EntraAuthService>();

            AuthProviderOptions authProviderOptions = GetValidatedAuthProviderOptions(builder);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("Entra", options =>
            {
                options.Authority = $"{authProviderOptions.Entra.BaseUri}/{authProviderOptions.Entra.Tenant}/v2.0";
                options.ClientId = authProviderOptions.Entra.ClientId;
                options.ClientSecret = authProviderOptions.Entra.ClientSecret;
                options.ResponseType = "code";
                options.SaveTokens = true;
            })
            .AddOpenIdConnect("OneLogin", options =>
            {
                options.Authority = authProviderOptions.OneLogin.BaseUri;
                options.ClientId = authProviderOptions.OneLogin.ClientId;
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.Scope.Clear();
                foreach (string scope in authProviderOptions.OneLogin.Scopes)
                {
                    options.Scope.Add(scope);
                }
                options.CallbackPath = "/authentication/callback";

                options.Events.OnRedirectToIdentityProvider = context =>
                {
                    context.ProtocolMessage.ResponseMode = "query";
                    context.ProtocolMessage.SetParameter("ui_locales", "en");
                    context.ProtocolMessage.SetParameter("vtr", "[\"Cl.Cm\"]");
                    return Task.CompletedTask;
                };

                options.Events.OnAuthorizationCodeReceived = async context =>
                {
                    OneLoginAuthService oneLoginAuthService = context.HttpContext.RequestServices.GetRequiredService<OneLoginAuthService>();
                    await oneLoginAuthService.AuthorizationCodeReceivedAsync(context);
                };

                options.Events.OnTokenValidated = async context =>
                {
                    OneLoginAuthService oneLoginAuthService = context.HttpContext.RequestServices.GetRequiredService<OneLoginAuthService>();
                    await oneLoginAuthService.TokenValidatedAsync(context);
                };

                options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
                {
                    OneLoginAuthService oneLoginAuthService = context.HttpContext.RequestServices.GetRequiredService<OneLoginAuthService>();
                    await oneLoginAuthService.RedirectToIdentityProviderForSignOutAsync(context);
                };

                options.Events.OnRemoteFailure = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var error = new { error = "Authentication failed", message = context.Failure?.Message ?? "Unknown error" };
                    context.Response.WriteAsJsonAsync(error);

                    context.HandleResponse();
                    return Task.CompletedTask;
                };
            });

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                _ = app.MapOpenApi();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        /// <summary>
        /// Retrieves and validates the <see cref="AuthProviderOptions"/> configuration section.
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/> containing the application configuration.</param>
        /// <returns>A validated <see cref="AuthProviderOptions"/> instance.</returns>
        /// <exception cref="ValidationException">
        /// Thrown if the <see cref="AuthProviderOptions"/> configuration is invalid.
        /// </exception>
        private static AuthProviderOptions GetValidatedAuthProviderOptions(WebApplicationBuilder builder)
        {
            AuthProviderOptions authProviderOptions = new();
            builder.Configuration.GetSection("AuthProviderOptions").Bind(authProviderOptions);

            ValidationContext validationContext = new(authProviderOptions);
            List<ValidationResult> results = new();
            bool isValid = Validator.TryValidateObject(authProviderOptions, validationContext, results, true);

            if (!isValid)
            {
                throw new ValidationException("AuthProviderOptions configuration is invalid: " +
                    string.Join("; ", results.Select(r => r.ErrorMessage)));
            }

            return authProviderOptions;
        }
    }
}
