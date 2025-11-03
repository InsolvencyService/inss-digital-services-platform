using System.Diagnostics.CodeAnalysis;

namespace INSS.Platform.Auth.Contracts.Request
{
    /// <summary>
    /// Represents a request to sign in a user, including post sign-in redirect information.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification ="DTO's, there is no business logic to test.")]
    public class SignInRequest
    {
        /// <summary>
        /// Gets or sets the URI to redirect to after a successful sign-in.
        /// </summary>
        public string PostSignInRedirectUri { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional user identifier for the sign-in request.
        /// </summary>
        public string? UserId { get; set; }
    }
}
