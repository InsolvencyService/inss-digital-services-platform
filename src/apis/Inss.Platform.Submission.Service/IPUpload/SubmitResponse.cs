using System.Net;

namespace Inss.Platform.Submission.Service.IPUpload;

public sealed class SubmitResponse
{
    public HttpStatusCode StatusCode { get; init; }
    
    public string? Error { get; init; }
}