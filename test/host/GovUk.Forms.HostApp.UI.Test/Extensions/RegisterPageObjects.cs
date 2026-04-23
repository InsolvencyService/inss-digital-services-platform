using GovUk.Forms.HostApp.UI.Test.Pages;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Pages.Declaration;
using GovUk.Forms.HostApp.UI.Test.Pages.Login;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class RegisterPageObjects
{
    public static void AddPageObjects(this IServiceCollection services)
    {
        services.AddScoped<IDeclarationPage, DeclarationPage>();
        services.AddScoped<ISection187Page, Section187Page>();
        services.AddScoped<ICommonPage, CommonPage>();
        services.AddScoped<IStartPage, StartPage>();
        services.AddScoped<ISignInPage, SignInPage>();
        services.AddScoped<IUploadDocumentPage, UploadDocumentPage>();
        services.AddScoped<DirectorConductReportingServicePage>();
    }
}
