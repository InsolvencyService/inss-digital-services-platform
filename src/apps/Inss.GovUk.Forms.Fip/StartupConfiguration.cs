using GovUk.Forms.Components;
using GovUk.Forms.Components.Resolvers;
using Inss.GovUk.Forms.Fip.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Inss.GovUk.Forms.Fip.StartupConfiguration))]

namespace Inss.GovUk.Forms.Fip;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            WebRoot webRoot = new();
            services.AddSingleton<IWebRoot>(webRoot);
            
            FipFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);

            services.AddSingleton<IStartPageResolver, StartPageResolver>();
        });
    }
}