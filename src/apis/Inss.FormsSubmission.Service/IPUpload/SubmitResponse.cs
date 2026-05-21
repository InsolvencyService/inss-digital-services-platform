using System.Net;

namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class SubmitResponse
{
    public HttpStatusCode StatusCode { get; init; }
    
    public string? Error { get; init; }
}