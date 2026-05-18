namespace Inss.FormsSubmission.Service.Endpoints;

public static class HealthEndpointHandler
{
    public static RouteHandlerBuilder HealthEndpoint(this IEndpointRouteBuilder builder)
    {
        return builder.MapGet("/health", () => Results.Ok("Healthy."));
    }
}