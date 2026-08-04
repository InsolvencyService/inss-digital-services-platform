using Inss.Platform.Domain;

namespace Inss.Platform.Fip.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public PagePathList BuildApp()
        {
            FipAppBuilder appBuilder = new();
            PagePathList pagePaths = [];
            pagePaths.AddRange(appBuilder.Build(services));
            return pagePaths;
        }
    }
}