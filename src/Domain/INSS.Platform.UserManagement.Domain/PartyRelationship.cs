namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents a relationship between two parties within the system.
/// </summary>
public class PartyRelationship : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the party initiating the relationship.
    /// </summary>
    public Guid FromPartyId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the party receiving the relationship.
    /// </summary>
    public Guid ToPartyId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the relationship type.
    /// </summary>
    public Guid RelationshipTypeId { get; set; }

    /// <summary>
    /// Gets or sets the date when the relationship started.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the relationship ended, if applicable.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Navigation property for the party initiating the relationship.
    /// </summary>
    public Party? FromParty { get; set; }

    /// <summary>
    /// Navigation property for the party receiving the relationship.
    /// </summary>
    public Party? ToParty { get; set; }

    /// <summary>
    /// Navigation property for the relationship type.
    /// </summary>
    public RelationshipType? RelationshipType { get; set; }
}
