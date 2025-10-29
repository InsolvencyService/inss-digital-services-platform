using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents the association between an organisation and a user.
    /// </summary>
    public class OrganisationUser : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the organisation.
        /// </summary>
        [Required]
        public Guid OrganisationId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }
    }
}
