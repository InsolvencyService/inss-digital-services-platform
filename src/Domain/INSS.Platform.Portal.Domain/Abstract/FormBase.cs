namespace INSS.Platform.Portal.Domain.Abstract;

/// <summary>
/// Represents the base class for all form types within the application.
/// </summary>
/// <remarks>
/// Inherits from <see cref="FormItemBase"/> and provides common properties for form identification and completion status.
/// </remarks>
public abstract class FormBase : FormItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormBase"/> class.
    /// Sets the <see cref="FormType"/> property to the assembly qualified name of the derived type.
    /// </summary>
    public FormBase()
    {
        FormType = GetType().AssemblyQualifiedName ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the unique identifier for this form set.
    /// </summary>
    /// <value>
    /// A <see cref="Guid"/> representing the unique identifier for the form set.
    /// </value>
    public Guid FormSetId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the type of the form.
    /// </summary>
    /// <remarks>
    /// This property identifies the specific type of form represented by the instance.
    /// It is used to distinguish between different form models within the application.
    /// </remarks>
    /// <value>
    /// A <see cref="string"/> containing the assembly qualified name of the form type.
    /// </value>
    public string FormType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the form or form list is complete.
    /// </summary>
    /// <value>
    /// <c>true</c> if the form is complete; otherwise, <c>false</c>.
    /// </value>
    public bool IsComplete { get; set; }
}
