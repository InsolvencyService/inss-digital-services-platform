using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.UserManagement.Core.Entities
{
    /// <summary>
    /// Represents the association between an application and a role.
    /// Inherits audit properties from <see cref="AuditableEntity"/>.
    /// </summary>
    public class ApplicationRole : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the application.
        /// </summary>
        [Required]
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the role.
        /// </summary>
        [Required]
        public Guid RoleId { get; set; }
    }
}
