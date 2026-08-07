using Inss.Platform.Submission.Service.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddConfigOverrideIfExists();
builder.Services.AddAppServices(builder.Configuration, builder.Environment);
WebApplication app = builder.Build();
app.UseApi();
app.Run();