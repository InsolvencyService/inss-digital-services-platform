using System.Diagnostics.CodeAnalysis;

namespace INSS.Platform.Common.Auth.Contracts.Request
{
    [ExcludeFromCodeCoverage(Justification ="DTO's, there is no business logic to test.")]
    /// <summary>
    /// Represents a login request containing CSRF token, client URL, and user identifier.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the CSRF (Cross-Site Request Forgery) token for the login request.
        /// </summary>
        public string CsrfToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the client URL from which the login request originated.
        /// </summary>
        public string ClientUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user identifier for the login request.
        /// </summary>
        public string UserId { get; set; } = string.Empty;
    }
}
