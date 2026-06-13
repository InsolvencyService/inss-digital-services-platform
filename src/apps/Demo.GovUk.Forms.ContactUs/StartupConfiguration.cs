using Demo.GovUk.Forms.ContactUs.Application.Factories;
using Demo.GovUk.Forms.ContactUs.Builders;
using Demo.GovUk.Forms.ContactUs.Domain;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components;
using GovUk.Forms.Components.Binding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StartupConfiguration = Demo.GovUk.Forms.ContactUs.StartupConfiguration;

[assembly: HostingStartup(typeof(StartupConfiguration))]

namespace Demo.GovUk.Forms.ContactUs;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            WebRoot webRoot = new();
            services.AddSingleton<IWebRoot>(webRoot);
            services.AddSingleton<IFormFactory, ContactUsFormFactory>();
            services.AddKeyedSingleton<IContentBinder, FileContentBinder>(typeof(FileUploadModel).FullName);
            ContactUsFlowchart flowchartBuilder = new();
            flowchartBuilder.Construct(services);
        });
    }
}