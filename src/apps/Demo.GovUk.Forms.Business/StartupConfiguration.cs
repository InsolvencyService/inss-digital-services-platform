using Demo.GovUk.Forms.Business.Application.Factories;
using Demo.GovUk.Forms.Business.Builders;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Demo.GovUk.Forms.Business.StartupConfiguration))]

namespace Demo.GovUk.Forms.Business;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.AddDeveloperConfig<StartupConfiguration>();
        
        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<IFormFactory, BusinessFormFactory>();
            
            YourCreditorsAndDebtorsFlowchart yourCreditorsAndDebtorsFlowchart = new();
            yourCreditorsAndDebtorsFlowchart.Construct(services);
            
            YourEmployeesFlowchart yourEmployeesFlowchart = new();
            yourEmployeesFlowchart.Construct(services);
            
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