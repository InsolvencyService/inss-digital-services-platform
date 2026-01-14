using System.Text.Json.Serialization;

namespace INSS.Platform.Canonical.Domain;

/// <summary>
/// Represents an address associated with a user.
/// </summary>
public class Address : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the user associated with this address.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the first line of the address.
    /// </summary>
    public string AddressLine1 { get; set; }

    /// <summary>
    /// Gets or sets the optional second line of the address.
    /// </summary>
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets the town or city of the address.
    /// </summary>
    public string TownCity { get; set; }

    /// <summary>
    /// Gets or sets the optional county of the address.
    /// </summary>
    public string? County { get; set; }

    /// <summary>
    /// Gets or sets the postcode of the address.
    /// </summary>
    public string Postcode { get; set; }

    /// <summary>
    /// Gets or sets the user entity associated with this address.
    /// </summary>
    [JsonIgnore]
    public virtual User? User { get; set; }
}
