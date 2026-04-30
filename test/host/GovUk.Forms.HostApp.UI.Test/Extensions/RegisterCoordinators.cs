using GovUk.Forms.HostApp.UI.Test.Coordinators;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class RegisterCoordinators
{
    public static void AddCoordinators(this IServiceCollection services)
    {
        services.AddScoped<DeclarationCoordinator>();
        services.AddScoped<SignInCoordinator>();
        services.AddScoped<StartPageCoordinator>();
        services.AddScoped<UploadDocumentCoordinator>();
        services.AddScoped<CommonCoordinator>();
        services.AddScoped<UploadErrorDetailsCoordinator>();
        services.AddScoped<UploadDocumentSummaryCoordinator>();
    }
}
