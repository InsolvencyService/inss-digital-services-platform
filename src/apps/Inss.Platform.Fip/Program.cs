using Inss.Platform.Application.Extensions;
using Inss.Platform.Component.Extensions;
using Inss.Platform.Domain.Primitives;
using Inss.Platform.Fip;
using Inss.Platform.Infrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddComponents(builder.Configuration);

// TODO: Move?
FipAppPagesBuilder builder2 = new();
PagePath[] pagePaths =  builder2.Build(builder.Services);

WebApplication app = builder.Build();
app.UseComponents();
app.UsePageEngine(pagePaths);
app.Run();