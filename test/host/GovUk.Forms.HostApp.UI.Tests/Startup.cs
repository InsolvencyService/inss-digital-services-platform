using GovUk.Forms.HostApp.UI.Tests.Config.Driver;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.HostApp.UI.Tests;

public static class Startup
{

    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddScoped<IPlaywrightDriver, PlaywrightDriver>();
        return services;

    }
}
