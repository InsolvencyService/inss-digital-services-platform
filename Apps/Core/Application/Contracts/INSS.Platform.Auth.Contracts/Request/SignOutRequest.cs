using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace INSS.Platform.Auth.Contracts.Request
{
    /// <summary>
    /// Represents a request to sign out a user, including the URI to redirect to after sign-out.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "DTO's, there is no business logic to test.")]
    public class SignOutRequest
    {
        /// <summary>
        /// Gets or sets the URI to redirect the user to after sign-out is complete.
        /// </summary>
        [Required]
        public string PostSignOutRedirectUri { get; set; }
    }
}
