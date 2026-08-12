using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GovUk.Forms.Components.Extensions;

public static class WebHostBuilderExtensions
{
    extension(IWebHostBuilder builder)
    {
        public IWebHostBuilder AddDeveloperConfig<TStartup>() where TStartup : class, IHostingStartup
        {
            builder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                if (!context.HostingEnvironment.IsDevelopment())
                {
                    return;
                }

                string app = typeof(TStartup).Assembly.GetName().Name!.Split('.').Last().ToLower();
                string devAppSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings.{app}.json");

                if (File.Exists(devAppSettings))
                {
                    configurationBuilder.AddJsonFile(devAppSettings, optional: true);
                    configurationBuilder.AddUserSecrets<TStartup>();
                }
            });

            return builder;
        }
    }
}