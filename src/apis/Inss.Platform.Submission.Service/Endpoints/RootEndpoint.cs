using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Inss.Platform.Submission.Service.Endpoints;

public static class RootEndpoint
{
    public static RouteHandlerBuilder DefineRootEndpoint(this IEndpointRouteBuilder builder)
    {
        return builder.MapGet("/", () => Results.Ok("Submission Service."));
    }
}