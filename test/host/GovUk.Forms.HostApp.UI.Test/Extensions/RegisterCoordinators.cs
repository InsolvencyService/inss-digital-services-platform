using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Support;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class RegisterCoordinators
{
    private const string ApiUploadTag = "api-upload";

    public static void AddCoordinators(this IServiceCollection services)
    {
        services.AddScoped<DeclarationCoordinator>();
        services.AddScoped<SignInCoordinator>();
        services.AddScoped<StartPageCoordinator>();
        services.AddScoped<UploadDocumentCoordinator>();
        services.AddScoped<CommonCoordinator>();
        services.AddScoped<UploadErrorDetailsCoordinator>();
        services.AddScoped<CheckYourAnswersCoordinator>();
        services.AddScoped<SubmissionConfirmationCoordinator>();
        services.AddScoped<NavigationCoordinator>();
        services.AddScoped<CaseReferenceCoordinator>();
        services.AddScoped<IUploadPageCoordinator, UploadPageCoordinator>();
        services.AddScoped<IFileUploadCoordinator, FileUploadCoordinator>();
        services.AddScoped<Func<IRp14aFixtureBuilder>>(sp =>
        {
            ScenarioContext scenarioContext = sp.GetRequiredService<ScenarioContext>();
            bool isApiUpload = scenarioContext.ScenarioInfo.Tags
                .Contains(ApiUploadTag, StringComparer.OrdinalIgnoreCase);
            return isApiUpload
                ? () => new Rp14aApiFixtureBuilder()
                : () => new Rp14aSpreadsheetFixtureBuilder();
        });
        services.AddScoped<IRp14aScenarioCoordinator, Rp14aScenarioCoordinator>();
        services.AddScoped<IUploadVerificationCoordinator, UploadVerificationCoordinator>();
        services.AddScoped<IUploadNavigationCoordinator, UploadNavigationCoordinator>();
        services.AddScoped<Func<IRp14FixtureBuilder>>(sp =>
        {
            ScenarioContext scenarioContext = sp.GetRequiredService<ScenarioContext>();
            bool isApiUpload = scenarioContext.ScenarioInfo.Tags
                .Contains(ApiUploadTag, StringComparer.OrdinalIgnoreCase);
            return isApiUpload
                ? () => new Rp14ApiFixtureBuilder()
                : () => new Rp14SpreadsheetFixtureBuilder();
        });
        services.AddScoped<IRp14ScenarioCoordinator, Rp14ScenarioCoordinator>();
    }
}
