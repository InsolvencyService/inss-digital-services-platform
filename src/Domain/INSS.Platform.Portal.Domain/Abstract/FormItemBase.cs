namespace INSS.Platform.Portal.Domain.Abstract;

/// <summary>
/// Represents the base class for form and form list items, providing common properties such as a unique identifier and creation timestamp.
/// </summary>
public abstract class FormItemBase
{
    /// <summary>
    /// Gets or sets the unique identifier for the form list item.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the date and time when the object was created.
    /// </summary>
    /// <remarks>
    /// The value is set to the current UTC date and time when the object is instantiated.
    /// </remarks>
    public DateTime Created { get; set; } = DateTime.UtcNow;
}
