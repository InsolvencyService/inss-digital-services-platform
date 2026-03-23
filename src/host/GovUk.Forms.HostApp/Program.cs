using Azure.Monitor.OpenTelemetry.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry().UseAzureMonitor(); // TODO: Move to the components?

WebApplication app = builder.Build();

app.Run();