namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents the association between a product role and a resource permission.
    /// </summary>
    public class ProductRoleResourcePermission : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the associated product role.
        /// </summary>
        public Guid ProductRoleId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated resource permission.
        /// </summary>
        public Guid ResourcePermissionId { get; set; }
    }
}
