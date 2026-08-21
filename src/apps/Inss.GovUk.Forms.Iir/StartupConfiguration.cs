using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Cookies;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Infrastructure.Extensions;
using Inss.GovUk.Forms.Iir.Application.Factories;
using Inss.GovUk.Forms.Iir.Application.Services;
using Inss.GovUk.Forms.Iir.Builders;
using Inss.GovUk.Forms.Iir.Infrastructure.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Inss.GovUk.Forms.Iir.StartupConfiguration))]
namespace Inss.GovUk.Forms.Iir;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.AddDeveloperConfig<StartupConfiguration>();
        
        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<IFormFactory, IirFormFactory>();
            services.AddSingleton<ICookieListResolver, CookieListResolver>();
            
            IirFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
            
            services.AddSearch<SearchEnrichmentService>("IIRSearch");
            
            if (context.UseMock("IIRSearch"))
            {
                services.AddMockSearchInfrastructure<MockSearchClient>(context.Configuration, "IIRSearch");
            }
            else
            {
                services.AddSearchInfrastructure(context.Configuration, "IIRSearch");
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