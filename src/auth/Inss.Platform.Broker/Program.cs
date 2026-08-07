using Inss.Platform.Broker.Extensions;
using Inss.Platform.Component.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddConfigOverrideIfExists();
builder.Services.AddComponents(builder.Configuration);
builder.Services.AddBrokerAuthentication(builder.Configuration, builder.Environment);
builder.Services.AddAuthCodeStore(builder.Configuration);

WebApplication app = builder.Build();
app.UseComponents();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedHost |
        ForwardedHeaders.XForwardedProto
});
app.UsePageEngine([]);
app.Run();