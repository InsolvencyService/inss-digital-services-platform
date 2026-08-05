using Inss.Platform.Application.Extensions;
using Inss.Platform.Component.Extensions;
using Inss.Platform.Domain;
using Inss.Platform.Fip.Application.Services;
using Inss.Platform.Fip.Extensions;
using Inss.Platform.Fip.Infrastructure.Clients;
using Inss.Platform.Infrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddComponents(builder.Configuration);
PagePathList pagePaths = builder.Services.BuildApp();

// TODO: Move
builder.Services.AddSearch<SearchEnrichmentService>("FIPSearch");
            
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddMockSearchInfrastructure<MockSearchClient>(builder.Configuration, "FIPSearch");
}
else
{
    builder.Services.AddSearchInfrastructure(builder.Configuration, "FIPSearch");
}

WebApplication app = builder.Build();
app.UseComponents();
app.UsePageEngine(pagePaths);
app.Run();