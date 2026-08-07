using Inss.Common.IPUpload;
using Inss.Platform.Submission.Service.Endpoints.Security;
using Inss.Platform.Submission.Service.Extensions;
using Inss.Platform.Submission.Service.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Inss.Platform.Submission.Service.Endpoints;

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
                    ILogger<Program> logger,
                    CancellationToken cancellationToken) =>
                {
                    logger.SubmittingIPUpload();
                    SubmitIPUploadResponse response = await handler.HandleAsync(request, cancellationToken);
                    return Results.Ok(response);
                })
            .Accepts<SubmitIPUploadRequest>(System.Net.Mime.MediaTypeNames.Application.Json)
            .Produces<SubmitIPUploadResponse>(StatusCodes.Status202Accepted, System.Net.Mime.MediaTypeNames.Application.Json)
            .RequireAuthorization(Policies.SubmissionPolicy);
    }
}