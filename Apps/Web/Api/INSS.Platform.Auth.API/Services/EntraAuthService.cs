using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Provides authentication services for OneLogin integration.
    /// </summary>
    public class EntraAuthService : IAuthService
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EntraAuthService"/> class.
        /// </summary>
        public EntraAuthService()
        {
        }

        /// <inheritdoc/>
        public async Task AuthorizationCodeReceivedAsync(AuthorizationCodeReceivedContext context)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task TokenValidatedAsync(TokenValidatedContext context)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task RedirectToIdentityProviderForSignOutAsync(RedirectContext context)
        {
            throw new NotImplementedException();
        }
    }
}
