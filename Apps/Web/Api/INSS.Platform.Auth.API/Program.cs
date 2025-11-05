using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.API.Services;
using INSS.Platform.Auth.Contracts;
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

            builder.Services.AddScoped<IAuthenticationEventHandler, AuthenticationEventHandler>();

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

                options.Events.OnTokenValidated = async context =>
                {
                    IAuthenticationEventHandler authEventHandler = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationEventHandler>();
                    await authEventHandler.HandleTokenValidatedAsync(context, AuthenticationProvider.Entra).ConfigureAwait(false);
                };

                options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
                {
                    IAuthenticationEventHandler authEventHandler = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationEventHandler>();
                    await authEventHandler.HandleRedirectToIdentityProviderForSignOutAsync(context, entraOptions.SignOutCallbackPath, AuthenticationProvider.Entra).ConfigureAwait(false);
                };

                options.Events.OnRemoteFailure = async context =>
                {
                    IAuthenticationEventHandler authEventHandler = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationEventHandler>();
                    await authEventHandler.HandleRemoteFailureAsync(context, AuthenticationProvider.Entra).ConfigureAwait(false);
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
                    IAuthenticationEventHandler authEventHandler = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationEventHandler>();
                    await authEventHandler.HandleAuthorizationCodeReceivedAsync(context, AuthenticationProvider.OneLogin).ConfigureAwait(false);
                };

                options.Events.OnTokenValidated = async context =>
                {
                    IAuthenticationEventHandler authEventHandler = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationEventHandler>();
                    await authEventHandler.HandleTokenValidatedAsync(context, AuthenticationProvider.OneLogin).ConfigureAwait(false);
                };

                options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
                {
                    IAuthenticationEventHandler authEventHandler = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationEventHandler>();
                    await authEventHandler.HandleRedirectToIdentityProviderForSignOutAsync(context, oneLoginOptions.SignOutCallbackPath, AuthenticationProvider.OneLogin).ConfigureAwait(false);
                };

                options.Events.OnRemoteFailure = async context =>
                {
                    IAuthenticationEventHandler authEventHandler = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationEventHandler>();
                    await authEventHandler.HandleRemoteFailureAsync(context, AuthenticationProvider.OneLogin).ConfigureAwait(false);
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
