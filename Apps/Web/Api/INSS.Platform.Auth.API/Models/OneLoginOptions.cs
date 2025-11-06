using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Auth.API.Models
{
    /// <summary>
    /// Represents configuration options for OneLogin authentication.
    /// </summary>
    public class OneLoginOptions
    {
        /// <summary>
        /// Gets or sets the client identifier used for OneLogin authentication.
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the base URI for the OneLogin service.
        /// </summary>
        [Required]
        public string BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the token endpoint URI for obtaining authentication tokens.
        /// </summary>
        [Required]
        public string TokenUri { get; set; }

        /// <summary>
        /// Gets or sets the callback path used during the sign-in process with OneLogin.
        /// </summary>
        [Required]
        public string SignInCallbackPath { get; set; }

        /// <summary>
        /// Gets or sets the callback path used during the sign-out process with OneLogin.
        /// </summary>
        [Required]
        public string SignOutCallbackPath { get; set; }

        /// <summary>
        /// Gets or sets the list of scopes requested during authentication.
        /// </summary>
        [Required]
        public List<string> Scopes { get; set; } = new();

        /// <summary>
        /// Gets or sets the private key used for JWT signing.
        /// </summary>
        [Required]
        public string JwtPrivateKey { get; set; }
    }
}