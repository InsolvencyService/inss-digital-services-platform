using Demo.GovUk.Forms.Bankruptcy.Application.Factories;
using Demo.GovUk.Forms.Bankruptcy.Builders;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Components.Resolvers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Demo.GovUk.Forms.Bankruptcy.StartupConfiguration))]

namespace Demo.GovUk.Forms.Bankruptcy;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<IFormFactory, BankruptcyFormFactory>();
            
            YourBankruptcyFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);

            services.AddSingleton<IStartPageResolver, StartPageResolver>();
            
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