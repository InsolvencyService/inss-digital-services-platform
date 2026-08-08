using Inss.Platform.Domain;

namespace Inss.Platform.Ipus.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAppServices(IWebHostEnvironment environment, IConfiguration configuration)
        {
            return services;
        }
        
        public PagePathList BuildApp(IConfiguration configuration)
        {
            IpusAppBuilder appBuilder = new();
            PagePathList pagePaths = [];
            pagePaths.AddRange(appBuilder.Build(services, configuration));
            return pagePaths;
        }
    }
}