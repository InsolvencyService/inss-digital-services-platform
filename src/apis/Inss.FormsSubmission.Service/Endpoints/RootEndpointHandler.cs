namespace Inss.FormsSubmission.Service.Endpoints;

public static class RootEndpointHandler
{
    public static RouteHandlerBuilder RootEndpoint(this IEndpointRouteBuilder builder)
    {
        return builder.MapGet("/", () => Results.Ok("Submission Service."));
    }
}