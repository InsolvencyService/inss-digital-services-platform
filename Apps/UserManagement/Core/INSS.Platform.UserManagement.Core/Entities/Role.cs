using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.UserManagement.Core.Entities
{
    /// <summary>
    /// Represents a user role within the system, including its name and description.
    /// </summary>
    public class Role : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the role.
        /// </summary>
        [MaxLength(1024)]
        public string Description { get; set; } = string.Empty;
    }
}
