using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Inss.Platform.Submission.Service.Endpoints;

public static class HealthEndpoint
{
    public static RouteHandlerBuilder DefineHealthEndpoint(this IEndpointRouteBuilder builder)
    {
        return builder.MapGet("/health", () => Results.Ok("Healthy."));
    }
}