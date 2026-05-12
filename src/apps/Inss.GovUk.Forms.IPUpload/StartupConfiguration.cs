using System.Diagnostics.CodeAnalysis;
using GovUk.Forms.Components;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Components.Options;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Builders;
using Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;
using Inss.GovUk.Forms.IPUpload.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: HostingStartup(typeof(Inss.GovUk.Forms.IPUpload.StartupConfiguration))]

namespace Inss.GovUk.Forms.IPUpload;

[ExcludeFromCodeCoverage]
public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            WebRoot webRoot = new();
            services.AddSingleton<IWebRoot>(webRoot);
            
            services.AddTransient<ICaseReferenceService, CaseReferenceService>();
            
            ExternalApiOptions dynamicsOptions = context.Configuration.GetSection("Dynamics").Get<ExternalApiOptions>()!;
            services.AddTypedClient<ISubmitIPUploadSectionClient, SubmitIPUploadSectionClient>(dynamicsOptions);

            RpsApiOptions rpsOptions = context.Configuration.GetSection("Rps").Get<RpsApiOptions>()!;
            
            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddTypedClient<ICaseReferenceClient, MockCaseReferenceClient>(rpsOptions);
            }
            else
            {
                services.AddOptions<RpsApiOptions>()
                    .Bind(context.Configuration.GetSection("Rps"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
                services.AddTypedClient<ICaseReferenceClient, CaseReferenceClient>(rpsOptions);
            }
            
            services.AddTransient<ISubmitUploadedXmlService, SubmitUploadedXmlService>();
            
            IPUploadFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}