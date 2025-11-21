namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents a type of relationship within the user management domain.
    /// </summary>
    public class RelationshipType : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the relationship type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the relationship type.
        /// </summary>
        public string Description { get; set; }
    }
}
