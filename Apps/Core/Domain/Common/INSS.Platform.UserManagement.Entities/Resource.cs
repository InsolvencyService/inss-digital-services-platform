namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents a resource entity with a name and description.
    /// </summary>
    public class Resource : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the resource.
        /// </summary>
        public string Description { get; set; }
    }
}
