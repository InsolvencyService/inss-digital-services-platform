using System.Diagnostics.CodeAnalysis;

namespace INSS.Platform.Auth.Contracts.Request
{
    [ExcludeFromCodeCoverage(Justification = "DTO's, there is no business logic to test.")]
    /// <summary>
    /// Represents a request to log out a user by providing their ID token.
    /// </summary>
    public class LogoutRequest
    {
        /// <summary>
        /// Gets or sets the ID token of the user to be logged out.
        /// </summary>
        public string IdToken { get; set; } = string.Empty;
    }
}
