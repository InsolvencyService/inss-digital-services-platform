using INSS.Platform.Audit.Functions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()    // correct for isolated worker
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddSingleton<LogAnalyticsClient>();
    })
    .Build();

host.Run();