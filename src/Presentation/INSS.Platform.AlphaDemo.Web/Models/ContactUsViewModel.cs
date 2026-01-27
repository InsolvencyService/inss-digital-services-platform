namespace INSS.Platform.AlphaDemo.Web.Models;

/// <summary>
/// Represents a view model for capturing contact us form data.
/// </summary>
public class ContactUsViewModel
{
    /// <summary>
    /// Gets or sets the name of the person submitting the contact form.
    /// </summary>
    /// <value>
    /// The full name of the contact person, or <c>null</c> if not provided.
    /// </value>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the email address of the person submitting the contact form.
    /// </summary>
    /// <value>
    /// A valid email address for communication, or <c>null</c> if not provided.
    /// </value>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the subject line for the contact inquiry.
    /// </summary>
    /// <value>
    /// A brief description of the inquiry topic, or <c>null</c> if not provided.
    /// </value>
    public string? Subject { get; set; }
    
    /// <summary>
    /// Gets or sets the detailed message content of the contact inquiry.
    /// </summary>
    /// <value>
    /// The main body text containing the inquiry details, or <c>null</c> if not provided.
    /// </value>
    public string? Message { get; set; }
}
