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
            builder.Services.AddOptions<AuthenticationProviderOptions>()
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

            builder.Services.AddScoped<OneLoginAuthenticationService>();
            builder.Services.AddScoped<EntraAuthenticationService>();

            AuthenticationProviderOptions authProviderOptions = GetValidatedAuthProviderOptions(builder);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("Entra", options =>
            {
                EntraOptions entraOptions = authProviderOptions.Entra;
                options.Authority = $"{entraOptions.BaseUri}/{entraOptions.Tenant}/v2.0";
                options.ClientId = entraOptions.ClientId;
                options.ClientSecret = entraOptions.ClientSecret;
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.CallbackPath = entraOptions.SignInCallbackPath;
                options.Scope.Clear();
                foreach (string scope in entraOptions.Scopes)
                {
                    options.Scope.Add(scope);
                }

                options.Events.OnAuthorizationCodeReceived = async context =>
                {
                    EntraAuthenticationService entraAuthService = context.HttpContext.RequestServices.GetRequiredService<EntraAuthenticationService>();
                    await entraAuthService.AuthorizationCodeReceivedAsync(context).ConfigureAwait(false);
                };

                options.Events.OnTokenValidated = async context =>
                {
                    EntraAuthenticationService entraAuthService = context.HttpContext.RequestServices.GetRequiredService<EntraAuthenticationService>();
                    await entraAuthService.TokenValidatedAsync(context).ConfigureAwait(false);
                };

                options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
                {
                    EntraAuthenticationService entraAuthService = context.HttpContext.RequestServices.GetRequiredService<EntraAuthenticationService>();
                    await entraAuthService.RedirectToIdentityProviderForSignOutAsync(context).ConfigureAwait(false);
                };

                options.Events.OnRemoteFailure = async context =>
                {
                    EntraAuthenticationService entraAuthService = context.HttpContext.RequestServices.GetRequiredService<EntraAuthenticationService>();
                    await entraAuthService.RemoteFailureAsync(context).ConfigureAwait(false);
                };
            })

            .AddOpenIdConnect("OneLogin", options =>
            {
                OneLoginOptions oneLoginOptions = authProviderOptions.OneLogin;
                options.Authority = oneLoginOptions.BaseUri;
                options.ClientId = oneLoginOptions.ClientId;
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.CallbackPath = oneLoginOptions.SignInCallbackPath;
                options.Scope.Clear();
                foreach (string scope in oneLoginOptions.Scopes)
                {
                    options.Scope.Add(scope);
                }

                options.Events.OnRedirectToIdentityProvider = static context =>
                {
                    context.ProtocolMessage.ResponseMode = "query";
                    context.ProtocolMessage.SetParameter("ui_locales", "en");
                    context.ProtocolMessage.SetParameter("vtr", "[\"Cl.Cm\"]");
                    return Task.CompletedTask;
                };

                options.Events.OnAuthorizationCodeReceived = async context =>
                {
                    OneLoginAuthenticationService oneLoginAuthService = context.HttpContext.RequestServices.GetRequiredService<OneLoginAuthenticationService>();
                    await oneLoginAuthService.AuthorizationCodeReceivedAsync(context).ConfigureAwait(false);
                };

                options.Events.OnTokenValidated = async context =>
                {
                    OneLoginAuthenticationService oneLoginAuthService = context.HttpContext.RequestServices.GetRequiredService<OneLoginAuthenticationService>();
                    await oneLoginAuthService.TokenValidatedAsync(context).ConfigureAwait(false);
                };

                options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
                {
                    OneLoginAuthenticationService oneLoginAuthService = context.HttpContext.RequestServices.GetRequiredService<OneLoginAuthenticationService>();
                    await oneLoginAuthService.RedirectToIdentityProviderForSignOutAsync(context).ConfigureAwait(false);
                };

                options.Events.OnRemoteFailure = async context =>
                {
                    OneLoginAuthenticationService oneLoginAuthService = context.HttpContext.RequestServices.GetRequiredService<OneLoginAuthenticationService>();
                    await oneLoginAuthService.RemoteFailureAsync(context);
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
        /// Retrieves and validates the <see cref="AuthenticationProviderOptions"/> configuration section.
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/> containing the application configuration.</param>
        /// <returns>A validated <see cref="AuthenticationProviderOptions"/> instance.</returns>
        /// <exception cref="ValidationException">
        /// Thrown if the <see cref="AuthenticationProviderOptions"/> configuration is invalid.
        /// </exception>
        private static AuthenticationProviderOptions GetValidatedAuthProviderOptions(WebApplicationBuilder builder)
        {
            AuthenticationProviderOptions authProviderOptions = new();
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
