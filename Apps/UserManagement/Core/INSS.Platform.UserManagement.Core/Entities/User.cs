using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INSS.Platform.UserManagement.Core.Entities
{
    /// <summary>
    /// Represents a user entity with identity and personal information.
    /// </summary>
    public class User : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier for the user's identity.
        /// </summary>
        public Guid? UserIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the user identity associated with this user.
        /// </summary>
        [ForeignKey("UserIdentityId")]
        public virtual UserIdentity? UserIdentity { get; set; }
    }
}
