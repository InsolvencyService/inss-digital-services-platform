using GovUk.Forms.HostApp.UI.Tests.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.HostApp.UI.Tests.Extensions;

public static class RegisterPageObjects
{
    public static void AddPageObjects(this IServiceCollection services)
    {
        services.AddScoped<IStartPage, StartPage>();
    }
}
