namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents an authentication provider entity.
    /// </summary>
    public class AuthenticationProvider : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the authentication provider.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the authentication provider.
        /// </summary>
        public string Description { get; set; }
    }
}
