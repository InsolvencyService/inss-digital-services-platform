using System.Diagnostics.CodeAnalysis;
using INSS.Platform.UserManagement.Repository;
using Microsoft.EntityFrameworkCore;

namespace INSS.Platform.UserManagement.API
{
    /// <summary>
    /// Entry point for the User Management API service.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "There is no business logic in this class to test.")]
    public static class Program
    {
        /// <summary>
        /// Configures and runs the User Management API service.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddApplicationInsightsTelemetry();

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
            builder.Services.AddScoped<IOrganisationRepository, OrganisationRepository>();
            builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.MapOpenApi();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
