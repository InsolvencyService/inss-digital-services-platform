namespace Inss.FormsSubmission.Service.Endpoints;

public static class RootEndpoint
{
    public static RouteHandlerBuilder DefineRootEndpoint(this IEndpointRouteBuilder builder)
    {
        return builder.MapGet("/", () => Results.Ok("Submission Service."));
    }
}