using System.Diagnostics.CodeAnalysis;
using GovUk.Forms.Components;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Components.Options;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Builders;
using Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            
            ExternalApiOptions rpsOptions = context.Configuration.GetSection("Rps").Get<ExternalApiOptions>()!;
            services.AddTypedClient<ICaseReferenceClient, CaseReferenceClient>(rpsOptions);
            
            IPUploadFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}