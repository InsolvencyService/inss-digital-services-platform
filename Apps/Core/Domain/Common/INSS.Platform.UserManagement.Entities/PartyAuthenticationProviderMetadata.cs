namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents metadata for a party's authentication provider, including session data and provider identifiers.
    /// </summary>
    public class PartyAuthenticationProviderMetadata : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the party.
        /// </summary>
        public Guid PartyId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the authentication policy provider.
        /// </summary>
        public Guid AuthenticationPolicyProviderId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the authentication provider user.
        /// </summary>
        public string AuthenticationProviderUserId { get; set; }

        /// <summary>
        /// Gets or sets the session data associated with the authentication provider.
        /// </summary>
        public string? AuthenticationProviderSessionData { get; set; }
    }
}
