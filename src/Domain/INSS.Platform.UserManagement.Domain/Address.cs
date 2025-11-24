using System.Text.Json.Serialization;

namespace INSS.Platform.UserManagement.Domain;

/// <summary>
/// Represents a postal address associated with a party.
/// </summary>
public class Address : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the party associated with this address.
    /// </summary>
    public Guid PartyId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the address type.
    /// </summary>
    public Guid AddressTypeId { get; set; }

    /// <summary>
    /// Gets or sets the first line of the address.
    /// </summary>
    public string AddressLine1 { get; set; }

    /// <summary>
    /// Gets or sets the second line of the address.
    /// </summary>
    public string AddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets the third line of the address.
    /// </summary>
    public string AddressLine3 { get; set; }

    /// <summary>
    /// Gets or sets the postcode for the address.
    /// </summary>
    public string Postcode { get; set; }

    /// <summary>
    /// Gets or sets the Unique Property Reference Number (UPRN) for the address.
    /// </summary>
    public long? UPRN { get; set; }

    /// <summary>
    /// Gets or sets the longitude coordinate of the address.
    /// </summary>
    public decimal? Longitude { get; set; }

    /// <summary>
    /// Gets or sets the latitude coordinate of the address.
    /// </summary>
    public decimal? Latitude { get; set; }

    /// <summary>
    /// Gets or sets the party associated with this address.
    /// </summary>
    [JsonIgnore]
    public virtual Party? Party { get; set; }
}
