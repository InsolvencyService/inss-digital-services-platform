using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Auth.API.Models
{
    /// <summary>
    /// Represents configuration options for the INSS authentication system.
    /// </summary>
    public class InssOptions
    {
        /// <summary>
        /// Gets or sets the issuer of the JWT token.
        /// </summary>
        [Required]
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the audience for which the JWT token is intended.
        /// </summary>
        [Required]
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the expiry duration (in minutes) for the JWT token.
        /// Must be greater than zero.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "ExpiryMinutes must be greater than zero.")]
        public int ExpiryMinutes { get; set; }

        /// <summary>
        /// Gets or sets the private key used to sign the JWT token.
        /// </summary>
        [Required]
        public string JwtPrivateKey { get; set; }
    }
}
