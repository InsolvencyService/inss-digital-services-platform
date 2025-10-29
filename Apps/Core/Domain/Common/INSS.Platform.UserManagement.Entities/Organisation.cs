using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents an organisation entity with audit information.
    /// </summary>
    public class Organisation : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the organisation.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }
}
