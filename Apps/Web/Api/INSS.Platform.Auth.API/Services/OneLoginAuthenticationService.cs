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
    public class OneLoginAuthenticationService : IAuthenticationService
    {
        private readonly ILogger<OneLoginAuthenticationService> _logger;
        private readonly IAuthenticationEventHandler _authEventHandlerHelper;
        private readonly AuthenticationProviderOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneLoginAuthenticationService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="appConfig">The application configuration.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public OneLoginAuthenticationService(
            ILogger<OneLoginAuthenticationService> logger,
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
            _logger.LogInformation("OneLogin Authentication - AuthorizationCodeReceivedAsync invoked.");

            await _authEventHandlerHelper
                .HandleAuthorizationCodeReceivedAsync(context, AuthenticationProvider.OneLogin)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task TokenValidatedAsync(TokenValidatedContext context)
        {
            _logger.LogInformation("OneLogin Authentication - TokenValidatedAsync invoked.");

            await _authEventHandlerHelper
                .HandleTokenValidatedAsync(context, AuthenticationProvider.OneLogin)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task RedirectToIdentityProviderForSignOutAsync(RedirectContext context)
        {
            _logger.LogInformation("OneLogin Authentication - RedirectToIdentityProviderForSignOutAsync invoked.");

            await _authEventHandlerHelper
                .HandleRedirectToIdentityProviderForSignOutAsync(context, _options.OneLogin.SignOutCallbackPath)
                .ConfigureAwait(false);

        }

        /// <inheritdoc/>
        public async Task RemoteFailureAsync(RemoteFailureContext context)
        {
            _logger.LogInformation("OneLogin Authentication - RemoteFailureAsync invoked.");

            await _authEventHandlerHelper
                .HandleRemoteFailureAsync(context)
                .ConfigureAwait(false);
        }
    }
}
