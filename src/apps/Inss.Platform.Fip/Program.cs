using Inss.Platform.Application.Extensions;
using Inss.Platform.Component.Extensions;
using Inss.Platform.Domain;
using Inss.Platform.Fip.Extensions;
using Inss.Platform.Infrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddComponents(builder.Configuration);
builder.Services.AddAppServices(builder.Environment, builder.Configuration);
PagePathList pagePaths = builder.Services.BuildApp();

WebApplication app = builder.Build();
app.UseComponents();
app.UsePageEngine(pagePaths);
app.Run();