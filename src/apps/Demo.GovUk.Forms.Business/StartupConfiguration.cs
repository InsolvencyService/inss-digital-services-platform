using Demo.GovUk.Forms.Business.Builders;
using GovUk.Forms.Components;
using GovUk.Forms.Components.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Demo.GovUk.Forms.Business.StartupConfiguration))]

namespace Demo.GovUk.Forms.Business;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            WebRoot webRoot = new();
            services.AddSingleton<IWebRoot>(webRoot);
            services.AddSingleton<IAuthenticationProvider, AnonymousAuthenticationProvider>();
            
            YourCreditorsAndDebtorsFlowchart yourCreditorsAndDebtorsFlowchart = new();
            yourCreditorsAndDebtorsFlowchart.Construct(services);
            
            YourEmployeesFlowchart yourEmployeesFlowchart = new();
            yourEmployeesFlowchart.Construct(services);
        });
    }
}