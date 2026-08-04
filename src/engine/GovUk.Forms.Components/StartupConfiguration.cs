using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(GovUk.Forms.Components.StartupConfiguration))]

namespace GovUk.Forms.Components;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureKestrel(options =>
        {
            options.AddServerHeader = false;
        });
    }
}