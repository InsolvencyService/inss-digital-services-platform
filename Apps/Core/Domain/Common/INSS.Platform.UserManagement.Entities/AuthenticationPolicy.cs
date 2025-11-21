namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents an authentication policy with configurable security requirements.
    /// </summary>
    public class AuthenticationPolicy : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the authentication policy.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the authentication policy.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multi-factor authentication is required.
        /// </summary>
        public bool RequireMultiFactorAuthentication { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether identity verification is required.
        /// </summary>
        public bool RequireIdentityVerification { get; set; }
    }
}
