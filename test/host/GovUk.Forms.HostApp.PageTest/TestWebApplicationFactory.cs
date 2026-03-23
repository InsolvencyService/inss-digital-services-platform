using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ReSharper disable ClassNeverInstantiated.Global

namespace GovUk.Forms.HostApp.PageTest;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private IHost? _host;
    
    public string ServerAddress  
    {  
        get 
        {  
            EnsureServer();  
            return ClientOptions.BaseAddress.ToString();  
        }  
    }
    
    protected override IHost CreateHost(IHostBuilder builder)  
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING", "InstrumentationKey=your-key-here");
        Environment.SetEnvironmentVariable("DOTNET_HOSTINGSTARTUPASSEMBLIES", "GovUk.Forms.Components;Demo.GovUk.Forms.AboutYou;" +
                                                                              "Demo.GovUk.Forms.Bankruptcy;" +
                                                                              "Demo.GovUk.Forms.Business;" +
                                                                              "Inss.GovUk.Forms.IPUpload");
        
        var testHost = builder.Build();  
        
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
        
        _host = builder.Build();  
        _host.Start();  
 
        var server = _host.Services.GetRequiredService<IServer>();  
        var addresses = server.Features.Get<IServerAddressesFeature>();  
 
        ClientOptions.BaseAddress = addresses!.Addresses.Select(a => new Uri(a)).Last();  
 
        testHost.Start();  
        
        return testHost;  
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(SetConfiguration);
        builder.ConfigureServices(OverrideServices);
    }
    
    private static void OverrideServices(IServiceCollection services)
    {
        // Use to configure services to override in the webapp such as dbs etc
    }
    
    private static void SetConfiguration(IConfigurationBuilder builder)
    {
        // Example config
        
        // var settings = new Dictionary<string, string?>
        // {
        //     { "Key", "Value" },
        // };
        //
        // builder.AddInMemoryCollection(settings);
    }
    
    private void EnsureServer()  
    {  
        if (_host is null)  
        {  
            // This forces WebApplicationFactory to bootstrap the server  
            using var _ = CreateDefaultClient();  
        }  
    }
}