using GovUk.Forms.Application.Services;
using GovUk.Forms.Components;
using GovUk.Forms.Components.Authentication;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Components.Options;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.Providers;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Builders;
using Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;
using Inss.GovUk.Forms.IPUpload.Infrastructure.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Inss.GovUk.Forms.IPUpload.StartupConfiguration))]

namespace Inss.GovUk.Forms.IPUpload;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            WebRoot webRoot = new();
            services.AddSingleton<IWebRoot>(webRoot);
            
            services.AddKeyedSingleton<ISubmitSectionService, FileUploadSubmitSectionService>(
                new ContentPath($"{webRoot.Root}/redundancy-payment"));
            services.AddSingleton<IXsdProvider, XsdProvider>();
            services.AddSingleton<IRedundancyPaymentProvider, RedundancyPaymentProvider>();
            ExternalApiOptions dynamicsOptions = context.Configuration.GetSection("Dynamics").Get<ExternalApiOptions>()!;
            services.AddTypedClient<ISubmitIPUploadSectionClient, SubmitIPUploadSectionClient>(dynamicsOptions);
            
            services.AddSingleton<IAuthenticationProvider, RpsAuthenticationProvider>();
            
            IPUploadFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}