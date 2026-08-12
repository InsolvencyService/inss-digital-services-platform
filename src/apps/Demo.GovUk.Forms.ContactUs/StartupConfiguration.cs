using Demo.GovUk.Forms.ContactUs.Application.Factories;
using Demo.GovUk.Forms.ContactUs.Builders;
using Demo.GovUk.Forms.ContactUs.Infrastructure.Clients;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Infrastructure.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StartupConfiguration = Demo.GovUk.Forms.ContactUs.StartupConfiguration;

[assembly: HostingStartup(typeof(StartupConfiguration))]

namespace Demo.GovUk.Forms.ContactUs;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.AddDeveloperConfig<StartupConfiguration>();
        
        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<IFormFactory, ContactUsFormFactory>();
            
            ContactUsFlowchart contactUsBuilder = new();
            contactUsBuilder.Construct(services);

            services.AddSearch("FindPerson");
            
            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddMockSearchInfrastructure<MockSearchClient>(context.Configuration, "FindPerson");
            }
            else
            {
                services.AddSearchInfrastructure(context.Configuration, "FindPerson");
            }
            
            FindPeopleFlowchart findPeopleFlowchart = new();
            findPeopleFlowchart.Construct(services);
            
            ContactUsFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
            
            services.AddComponents(context.Configuration);
            services.AddFormEngine(context.Configuration);
        });
        
        builder.Configure(app =>
        {
            app.UseComponents();
            app.UseFormEngine();
        });
    }
}