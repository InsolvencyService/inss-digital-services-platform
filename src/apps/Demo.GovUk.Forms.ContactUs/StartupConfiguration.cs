using Demo.GovUk.Forms.ContactUs.Application.Factories;
using Demo.GovUk.Forms.ContactUs.Builders;
using Demo.GovUk.Forms.ContactUs.Factories;
using GovUk.Forms.Components;
using GovUk.Forms.Components.Factories;
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
            services.AddSingleton<IFormFactory, ContactUsFormFactory>();
            
            ContactUsFlowchart contactUsBuilder = new();
            contactUsBuilder.Construct(services);
            
            FindPeopleFlowchart findPeopleFlowchart = new();
            findPeopleFlowchart.Construct(services);
        });
    }
}