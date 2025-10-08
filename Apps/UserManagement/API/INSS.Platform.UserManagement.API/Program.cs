using INSS.Platform.UserManagement.Repository;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<UserManagementDbContext>(options =>
{
    ConfigurationManager configuration = builder.Configuration;
    string? sqlConnectionString = configuration.GetConnectionString("UserManagementDb");

    if (string.IsNullOrEmpty(sqlConnectionString))
    {
        throw new InvalidOperationException("SQL Server connection string is not configured.  Update appsettings.json or set the environment variable (ConnectionStrings__UserManagementDb)");
    }

    options.UseSqlServer(sqlConnectionString);
});

builder.Services.AddScoped<IUserRepository, UserRepository>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
