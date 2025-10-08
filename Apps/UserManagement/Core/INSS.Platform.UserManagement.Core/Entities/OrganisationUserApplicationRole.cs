using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.UserManagement.Core.Entities
{
    /// <summary>
    /// Represents the association between an organisation user and an application role.
    /// </summary>
    public class OrganisationUserApplicationRole : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the organisation user.
        /// </summary>
        [Required]
        public Guid OrganisationUserId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the application role.
        /// </summary>
        [Required]
        public Guid ApplicationRoleId { get; set; }
    }
}
