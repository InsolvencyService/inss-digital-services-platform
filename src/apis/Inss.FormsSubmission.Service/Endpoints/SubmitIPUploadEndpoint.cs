using Inss.Common.IPUpload;
using Inss.FormsSubmission.Service.Endpoints.Security;
using Inss.FormsSubmission.Service.Extensions;
using Inss.FormsSubmission.Service.Handlers;

namespace Inss.FormsSubmission.Service.Endpoints;

public static class SubmitIPUploadEndpoint
{
    public static RouteHandlerBuilder DefineSubmitIPUploadEndpoint(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapPost(
                "/ipupload/submit",
                async (
                    SubmitIPUploadRequest request, 
                    IHandler<SubmitIPUploadRequest, SubmitIPUploadResponse> handler,
                    ILogger<Program> logger) =>
                {
                    logger.SubmittingIPUpload();
                    SubmitIPUploadResponse response = await handler.HandleAsync(request);
                    return Results.Ok(response);
                })
            .Accepts<SubmitIPUploadRequest>(System.Net.Mime.MediaTypeNames.Application.Json)
            .Produces<SubmitIPUploadResponse>(StatusCodes.Status202Accepted, System.Net.Mime.MediaTypeNames.Application.Json)
            .RequireAuthorization(Policies.SubmissionPolicy);
    }
}