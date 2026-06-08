// ReSharper disable UnusedAutoPropertyAccessor.Global - Config

using System.ComponentModel.DataAnnotations;

namespace Inss.FormsSubmission.Service.Options;

public sealed class NotifyOptions
{
    [Required]
    public string ApiKey { get; init; }
    
    [Required]
    public string IPUploadTemplateId { get; init; }
    
    public string? IPUploadBccEmail { get; init; }
}