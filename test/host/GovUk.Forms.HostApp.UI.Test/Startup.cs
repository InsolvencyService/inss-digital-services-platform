using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Config.Environments;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.HostApp.UI.Test;

public static class Startup
{

    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddScoped(_ =>
        {
            string testName = TestContext.CurrentContext.Test.Name;
            TestEnvironment environment = EnvironmentConfigFactory.CurrentEnvironment;
            string workDirectory = TestContext.CurrentContext.WorkDirectory;

            return new TestArtifacts(testName, environment, workDirectory);
        });
        services.AddScoped<IPlaywrightDriver, PlaywrightDriver>();
        services.AddPageObjects();
        services.AddCoordinators();
        return services;

    }
}
