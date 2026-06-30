using Demo.GovUk.Forms.ContactUs.Application.Factories;
using Demo.GovUk.Forms.ContactUs.Builders;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Infrastructure.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StartupConfiguration = Demo.GovUk.Forms.ContactUs.StartupConfiguration;

[assembly: HostingStartup(typeof(StartupConfiguration))]

namespace Demo.GovUk.Forms.ContactUs;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<IFormFactory, ContactUsFormFactory>();
            
            ContactUsFlowchart contactUsBuilder = new();
            contactUsBuilder.Construct(services);
            
            services.AddSearch("Config1");
            services.AddSearchInfrastructure(context.Configuration, "Config1");
            
            services.AddSearch("Config2");
            services.AddSearchInfrastructure(context.Configuration, "Config2");
            
            FindPeopleFlowchart findPeopleFlowchart = new();
            findPeopleFlowchart.Construct(services);
            
            FindOtherPeopleFlowchart findOtherPeopleFlowchart = new();
            findOtherPeopleFlowchart.Construct(services);
            
            ContactUsFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}