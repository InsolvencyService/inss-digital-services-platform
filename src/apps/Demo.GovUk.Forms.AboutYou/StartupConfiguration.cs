using Demo.GovUk.Forms.AboutYou.Application.Factories;
using Demo.GovUk.Forms.AboutYou.Application.Services;
using Demo.GovUk.Forms.AboutYou.Builders;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Demo.GovUk.Forms.AboutYou.StartupConfiguration))]

namespace Demo.GovUk.Forms.AboutYou;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IFormFactory, AboutYouFormFactory>();
            services.AddKeyedTransient<IFormPrePopulationService, TestFormPrePopulationService>(WebInfo.Root);
            YourDetailsFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}