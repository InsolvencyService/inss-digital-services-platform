using GovUk.Forms.HostApp.UI.Tests.Pages;
using GovUk.Forms.HostApp.UI.Tests.Pages.Common;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.HostApp.UI.Tests.Extensions;

public static class RegisterPageObjects
{
    public static void AddPageObjects(this IServiceCollection services)
    {
        services.AddScoped<IDeclarationPage, DeclarationPage>();
        services.AddScoped<ISection187Page, Section187Page>();
        services.AddScoped<ICommonPage, CommonPage>();
    }
}
