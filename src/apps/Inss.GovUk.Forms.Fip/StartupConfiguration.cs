using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Resolvers;
using Inss.GovUk.Forms.Fip.Application.Factories;
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
            services.AddSingleton<IFormFactory, FipFormFactory>();
            
            FipFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);

            services.AddSingleton<IStartPageResolver, StartPageResolver>();
        });
    }
}