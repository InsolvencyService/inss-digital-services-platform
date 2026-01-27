namespace INSS.Platform.AlphaDemo.Web.Models;

/// <summary>
/// Represents a view model for reporting issues in the INSS Forms Analytics system.
/// </summary>
public class ReportIssueViewModel
{
    /// <summary>
    /// Gets or sets the name of the person reporting the issue.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the email address of the person reporting the issue.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the area or category where the issue occurred.
    /// </summary>
    public string? Area { get; set; }
    
    /// <summary>
    /// Gets or sets the description of the issue being reported.
    /// </summary>
    public string? Issue { get; set; }
}
