using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
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
        services.AddScoped<CheckYourAnswersCoordinator>();
        services.AddScoped<IUploadPageCoordinator, UploadPageCoordinator>();
        services.AddScoped<IFileUploadCoordinator, FileUploadCoordinator>();
        services.AddScoped<IRp14aScenarioCoordinator, Rp14aScenarioCoordinator>();
        services.AddScoped<IUploadVerificationCoordinator, UploadVerificationCoordinator>();
        services.AddScoped<IUploadNavigationCoordinator, UploadNavigationCoordinator>();
    }
}
