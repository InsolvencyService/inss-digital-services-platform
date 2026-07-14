// ReSharper disable UnusedAutoPropertyAccessor.Global - Config

using System.ComponentModel.DataAnnotations;

namespace Inss.FormsSubmission.Service.Options;

public sealed class NotifyOptions
{
    [Required]
    public string ApiKey { get; init; }
    
    [Required]
    public string IPUploadExternalTemplateId { get; init; }
    
    [Required]
    public string IPUploadInternalTemplateId { get; init; }
    
    [Required]
    public string IPUploadInternalEmail { get; init; }
}