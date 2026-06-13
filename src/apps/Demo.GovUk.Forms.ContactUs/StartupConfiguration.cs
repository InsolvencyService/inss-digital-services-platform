using Demo.GovUk.Forms.ContactUs.Application.Factories;
using Demo.GovUk.Forms.ContactUs.Builders;
using GovUk.Forms.Application.Factories;
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
            services.AddSingleton<IFormFactory, ContactUsFormFactory>();
            ContactUsFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}