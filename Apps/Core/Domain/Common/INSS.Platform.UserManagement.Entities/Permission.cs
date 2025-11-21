namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents a permission that can be assigned to a user or role.
    /// </summary>
    public class Permission : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the permission.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the permission.
        /// </summary>
        public string Description { get; set; }
    }
}
