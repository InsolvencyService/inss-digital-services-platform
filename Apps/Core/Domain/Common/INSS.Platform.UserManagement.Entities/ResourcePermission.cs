namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents the association between a resource and a permission within the user management system.
    /// </summary>
    public class ResourcePermission : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the resource.
        /// </summary>
        public Guid ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the permission.
        /// </summary>
        public Guid PermissionId { get; set; }
    }
}
