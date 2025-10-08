using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.UserManagement.Core.Entities
{
    /// <summary>
    /// Represents a user identity associated with an identity provider.
    /// </summary>
    public class UserIdentity : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique user identifier provided by the identity provider.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string IdentityProviderUserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier of the identity provider.
        /// </summary>
        [Required]
        public Guid IdentityProviderId { get; set; }
    }
}
