using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Audit.Application.Options;

/// <summary>
/// Represents configuration options for audit publishing.
/// </summary>
public class AuditOptions
{
    /// <summary>
    /// Gets or sets the endpoint for the audit topic.
    /// </summary>
    [Required]
    public string TopicEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the access key used for authentication to the audit topic.
    /// </summary>
    /// <remarks>
    /// NOTE: In the production environment, use Managed Identity for authentication.
    /// </remarks>
    [Required]
    public string AccessKey { get; set; }
   
    /// <summary>
    /// Gets or sets the prefix for audit message subjects.
    /// </summary>
    [Required]
    public string SubjectPrefix { get; set; }
}
