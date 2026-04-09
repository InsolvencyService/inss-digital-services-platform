using GovUk.Forms.HostApp.UI.Tests.Coordinators;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.HostApp.UI.Tests.Extensions;

public static class RegisterCoordinators
{
    public static void AddCoordinators(this IServiceCollection services)
    {
        services.AddScoped<DemoCoordinator>();
    }
}
