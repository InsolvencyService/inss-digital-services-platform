namespace Inss.FormsSubmission.Service.Endpoints;

public static class HealthEndpoint
{
    public static RouteHandlerBuilder DefineHealthEndpoint(this IEndpointRouteBuilder builder)
    {
        return builder.MapGet("/health", () => Results.Ok("Healthy."));
    }
}