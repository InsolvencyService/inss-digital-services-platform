using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.API.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
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

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddApplicationInsightsTelemetry();

            builder.Services.AddOptions<AuthenticationProviderOptions>()
                .Bind(builder.Configuration.GetSection("AuthProviderOptions"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddScoped<IAuthenticationEventHandler, AuthenticationEventHandler>();

            ConfigureAuthenticationProviders(builder);

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
        /// Configures the authentication providers for the application.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="WebApplicationBuilder"/> used to configure services and authentication.
        /// </param>
        private static void ConfigureAuthenticationProviders(WebApplicationBuilder builder)
        {
            AuthenticationProviderOptions authProviderOptions = GetValidatedAuthProviderOptions(builder);

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "Platform.Auth.Api";
            })
            .AddEntraOpenIdConnect(authProviderOptions.Entra)
            .AddOneLoginOpenIdConnect(authProviderOptions.OneLogin);
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
