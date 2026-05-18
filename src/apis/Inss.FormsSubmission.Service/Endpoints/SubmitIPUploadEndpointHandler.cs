using Inss.FormsSubmission.Service.Endpoints.Security;
using Inss.FormsSubmission.Service.Extensions;

namespace Inss.FormsSubmission.Service.Endpoints;

public static class SubmitIPUploadEndpointHandler
{
    public static RouteHandlerBuilder SubmitIPUploadEndpoint(this IEndpointRouteBuilder builder)
    {
        return builder.MapGet("/ipupload/submit", (ILogger<Program> logger) =>
        {
            // TODO:
            // 1. This needs to be a post
            // 2. See MEDS-1066 spike for flow details as part of the implementation
            
            logger.SubmittingIPUpload();
            
            return Results.Ok("TODO.");
        }).RequireAuthorization(Policies.SubmissionPolicy);
    }
}