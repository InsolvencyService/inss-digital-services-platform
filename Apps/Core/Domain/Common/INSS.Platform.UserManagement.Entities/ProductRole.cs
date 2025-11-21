namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents the association between a product and a role within the user management system.
    /// </summary>
    public class ProductRole : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the associated product.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated role.
        /// </summary>
        public Guid RoleId { get; set; }
    }
}
