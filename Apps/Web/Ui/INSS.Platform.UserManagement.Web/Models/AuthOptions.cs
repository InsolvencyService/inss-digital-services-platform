using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.UserManagement.Web.Models
{
    /// <summary>
    /// Represents the authentication configuration options for the application.
    /// </summary>
    public class AuthOptions
    {
        /// <summary>
        /// Gets or sets the base URL of the API used for authentication.
        /// </summary>
        [Required]
        public string BaseApiUrl { get; set; }

        /// <summary>
        /// Gets or sets the expected JWT issuer.
        /// </summary>
        [Required]
        public string JwtIssuer { get; set; }

        /// <summary>
        /// Gets or sets the expected JWT audience.
        /// </summary>
        [Required]
        public string JwtAudience { get; set; }

        /// <summary>
        /// Gets or sets the public key used to validate JWT tokens.
        /// </summary>
        [Required]
        public string JwtPublicKey { get; set; }

        /// <summary>
        /// Gets or sets the authentication provider name.
        /// </summary>
        [Required]
        public string AuthProvider { get; set; }
    }
}
