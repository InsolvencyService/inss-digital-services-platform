using GovUk.Forms.HostApp.UI.Tests.Config.Driver;
using GovUk.Forms.HostApp.UI.Tests.Extensions;
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
        services.AddPageObjects();
        services.AddCoordinators();
        return services;

    }
}
