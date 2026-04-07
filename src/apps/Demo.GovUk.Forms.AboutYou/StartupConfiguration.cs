using Demo.GovUk.Forms.AboutYou.Application.Services;
using Demo.GovUk.Forms.AboutYou.Builders;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Components;
using GovUk.Forms.Components.Authentication;
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
            WebRoot webRoot = new();
            services.AddSingleton<IWebRoot>(webRoot);
            services.AddKeyedSingleton<IFormPrePopulationService, TestFormPrePopulationService>(webRoot.Root);
            services.AddSingleton<IAuthenticationProvider, AnonymousAuthenticationProvider>();
            YourDetailsFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}