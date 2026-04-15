using GovUk.Forms.HostApp.UI.Test.Pages;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class RegisterPageObjects
{
    public static void AddPageObjects(this IServiceCollection services)
    {
        services.AddScoped<IDeclarationPage, DeclarationPage>();
        services.AddScoped<ISection187Page, Section187Page>();
        services.AddScoped<ICommonPage, CommonPage>();
    }
}
