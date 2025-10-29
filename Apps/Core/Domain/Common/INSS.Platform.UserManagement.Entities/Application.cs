using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents an application entity within the user management system.
    /// Inherits audit properties from <see cref="AuditableEntity"/>.
    /// </summary>
    public class Application : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// Maximum length is 255 characters.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier of the associated identity provider.
        /// </summary>
        [Required]
        public Guid IdentityProviderId { get; set; }
    }
}
