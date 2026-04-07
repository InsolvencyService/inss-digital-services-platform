using Demo.GovUk.Forms.Bankruptcy.Builders;
using GovUk.Forms.Components;
using GovUk.Forms.Components.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Demo.GovUk.Forms.Bankruptcy.StartupConfiguration))]

namespace Demo.GovUk.Forms.Bankruptcy;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            WebRoot webRoot = new();
            services.AddSingleton<IWebRoot>(webRoot);
            services.AddSingleton<IAuthenticationProvider, AnonymousAuthenticationProvider>();
            
            YourBankruptcyFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}