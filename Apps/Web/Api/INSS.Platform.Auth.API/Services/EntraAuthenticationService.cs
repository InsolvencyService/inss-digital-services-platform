using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Provides authentication services for OneLogin integration.
    /// </summary>
    public class EntraAuthenticationService : IAuthenticationService
    {
        private readonly ILogger<EntraAuthenticationService> _logger;
        private readonly IAuthenticationEventHandler _authEventHandlerHelper;
        private readonly AuthenticationProviderOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntraAuthenticationService"/> class.
        /// </summary>
        public EntraAuthenticationService(
            ILogger<EntraAuthenticationService> logger, 
            IAuthenticationEventHandler authEventHandlerHelper, 
            IOptions<AuthenticationProviderOptions> options)
        {
            _logger = logger;
            _authEventHandlerHelper = authEventHandlerHelper;
            _options = options.Value;
        }

        /// <inheritdoc/>
        public async Task AuthorizationCodeReceivedAsync(AuthorizationCodeReceivedContext context)
        {
            _logger.LogInformation("Entra Authentication - AuthorizationCodeReceivedAsync invoked.");
            
            return;
        }

        /// <inheritdoc/>
        public async Task TokenValidatedAsync(TokenValidatedContext context)
        {
            _logger.LogInformation("Entra Authentication - TokenValidatedAsync invoked.");

            await _authEventHandlerHelper
                .HandleTokenValidatedAsync(context, AuthenticationProvider.Entra)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task RedirectToIdentityProviderForSignOutAsync(RedirectContext context)
        {
            _logger.LogInformation("Entra Authentication - RedirectToIdentityProviderForSignOutAsync invoked.");

            await _authEventHandlerHelper
                .HandleRedirectToIdentityProviderForSignOutAsync(context, _options.Entra.SignOutCallbackPath)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task RemoteFailureAsync(RemoteFailureContext context)
        {
            _logger.LogInformation("Entra Authentication - RemoteFailureAsync invoked.");

            await _authEventHandlerHelper
                .HandleRemoteFailureAsync(context)
                .ConfigureAwait(false);
        }
    }
}
