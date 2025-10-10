using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.UserManagement.Core.Entities
{
    /// <summary>
    /// Represents an identity provider configuration for authentication.
    /// </summary>
    public class IdentityProvider : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the identity provider.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the issuer URL of the identity provider.
        /// </summary>
        [MaxLength(2048)]
        public string? IssuerUrl { get; set; }

        /// <summary>
        /// Gets or sets the client ID used for authentication with the identity provider.
        /// </summary>
        [MaxLength(255)]
        public string? ClientId { get; set; }

        /// <summary>
        /// Gets or sets the secret used for authentication with the identity provider.
        /// </summary>
        [MaxLength(255)]
        public string? Secret { get; set; }
    }
}
