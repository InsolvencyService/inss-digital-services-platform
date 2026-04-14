using GovUk.Forms.HostApp.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddHostServices();
WebApplication app = builder.Build();
app.Run();