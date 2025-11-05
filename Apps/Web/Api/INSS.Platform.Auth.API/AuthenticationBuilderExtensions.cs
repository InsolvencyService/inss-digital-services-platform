using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.API.Services;
using INSS.Platform.Auth.Contracts;
using Microsoft.AspNetCore.Authentication;

namespace INSS.Platform.Auth.API
{
    /// <summary>
    /// Provides extension methods for configuring authentication providers using <see cref="AuthenticationBuilder"/>
    /// </summary>
    public static class AuthenticationBuilderExtensions
    {
        /// <summary>
        /// Adds and configures the Azure Entra OpenID Connect authentication provider.
        /// </summary>
        /// <param name="builder">The authentication builder to add the provider to.</param>
        /// <param name="entraOptions">The options used to configure the Entra provider.</param>
        /// <returns>The updated <see cref="AuthenticationBuilder"/> instance.</returns>
        public static AuthenticationBuilder AddEntraOpenIdConnect(
            this AuthenticationBuilder builder,
            EntraOptions entraOptions)
        {
            return builder.AddOpenIdConnect("Entra", options =>
            {
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
            });
        }

        /// <summary>
        /// Adds and configures the OneLogin OpenID Connect authentication provider.
        /// </summary>
        /// <param name="builder">The authentication builder to add the provider to.</param>
        /// <param name="oneLoginOptions">The options used to configure the OneLogin provider.</param>
        /// <returns>The updated <see cref="AuthenticationBuilder"/> instance.</returns>
        public static AuthenticationBuilder AddOneLoginOpenIdConnect(
            this AuthenticationBuilder builder,
            OneLoginOptions oneLoginOptions)
        {
            return builder.AddOpenIdConnect("OneLogin", options =>
            {
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
        }
    }
}