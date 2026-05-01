using Demo.GovUk.Forms.ContactUs.Builders;
using GovUk.Forms.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StartupConfiguration = Demo.GovUk.Forms.ContactUs.StartupConfiguration;

[assembly: HostingStartup(typeof(StartupConfiguration))]

namespace Demo.GovUk.Forms.ContactUs;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            WebRoot webRoot = new();
            services.AddSingleton<IWebRoot>(webRoot);
            ContactUsFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}