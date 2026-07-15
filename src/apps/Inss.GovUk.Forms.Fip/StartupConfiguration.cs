using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Infrastructure.Extensions;
using Inss.GovUk.Forms.Fip.Application.Factories;
using Inss.GovUk.Forms.Fip.Builders;
using Inss.GovUk.Forms.Fip.Infrastructure.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: HostingStartup(typeof(Inss.GovUk.Forms.Fip.StartupConfiguration))]

namespace Inss.GovUk.Forms.Fip;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<IFormFactory, FipFormFactory>();
            
            FipFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);

            services.AddSearch("FIPSearch");
            
            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddMockSearchInfrastructure<MockSearchClient>(context.Configuration, "FIPSearch");
            }
            else
            {
                services.AddSearchInfrastructure(context.Configuration, "FIPSearch");
            }
            
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