using Inss.Platform.Application.Extensions;
using Inss.Platform.Component.Extensions;
using Inss.Platform.Domain;
using Inss.Platform.Infrastructure.Extensions;
using Inss.Platform.RpsProvider.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddConfigOverrideIfExists();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddComponents(builder.Configuration);
builder.Services.AddRpsAuthentication(builder.Configuration, builder.Environment);
builder.Services.AddAuthCodeStore(builder.Configuration);
PagePathList pagePaths = builder.Services.BuildApp(builder.Configuration);

WebApplication app = builder.Build();
app.UseComponents();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedHost |
        ForwardedHeaders.XForwardedProto
});
app.UsePageEngine(pagePaths);
app.Run();