using Inss.Platform.Component.Extensions;
using Inss.Platform.RpsProvider.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddComponents(builder.Configuration);
builder.Services.AddRpsAuthentication(builder.Configuration, builder.Environment);
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