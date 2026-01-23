namespace INSS.Platform.Portal.Domain;

/// <summary>
/// Provides a base class for form models, including common properties such as instance identifier and user.
/// </summary>
/// <remarks>
/// This class should be marked as abstract to prevent instantiation, as its purpose is to be the base class for derived form types.
/// However it cannot be deserialized by CosmosDB if it is abstract.
/// </remarks>
public class FormBase
{
    public FormBase()
    {
        FormType = GetType().AssemblyQualifiedName ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the unique identifier for this form instance.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();


    /// <summary>
    /// Gets or sets the type of the form.
    /// </summary>
    /// <remarks>
    /// This property identifies the specific type of form represented by the instance.
    /// It is used to distinguish between different form models within the application.
    /// </remarks>
    public string FormType { get; set; }


    /// <summary>
    /// Gets or sets a value indicating whether the form is complete.
    /// </summary>
    /// <value>
    /// <c>true</c> if the form is complete; otherwise, <c>false</c>.
    /// </value>
    public bool IsComplete { get; set; }
}
