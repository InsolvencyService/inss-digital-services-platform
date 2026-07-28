using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Infrastructure.Extensions;
using Inss.GovUk.Forms.Eiir.Application.Factories;
using Inss.GovUk.Forms.Eiir.Builders;
using Inss.GovUk.Forms.Eiir.Infrastructure.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: HostingStartup(typeof(Inss.GovUk.Forms.Eiir.StartupConfiguration))]

namespace Inss.GovUk.Forms.Eiir;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<IFormFactory, EiirFormFactory>();
            
            EiirFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);

            services.AddSearch("EIIRPersonSearch");

            //if (context.HostingEnvironment.IsDevelopment())
            //{
            //    services.AddMockSearchInfrastructure<MockSearchClient>(context.Configuration, "EIIRPersonSearch");
            //}
            //else
            //{
            services.AddSearchInfrastructure(context.Configuration, "EIIRPersonSearch");
            //}
            
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