namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents a party entity with type and source of introduction information.
    /// </summary>
    public class Party : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the party type.
        /// </summary>
        public Guid PartyTypeId { get; set; }

        /// <summary>
        /// Gets or sets the source of introduction for the party.
        /// </summary>
        public string SourceOfIntroduction { get; set; }

        public virtual PartyType? PartyType { get; set; }

        /// <summary>
        /// Gets or sets the collection of addresses associated with the party.
        /// </summary>
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

        /// <summary>
        /// Gets or sets the relationships where this party is the source (from).
        /// </summary>
        public virtual ICollection<PartyRelationship> RelationshipsFrom { get; set; } = new List<PartyRelationship>();

        /// <summary>
        /// Gets or sets the relationships where this party is the target (to).
        /// </summary>
        public virtual ICollection<PartyRelationship> RelationshipsTo { get; set; } = new List<PartyRelationship>();
    }
}
