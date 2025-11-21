using System.Diagnostics.CodeAnalysis;

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
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddApplicationInsightsTelemetry();

            builder.Services.AddUserManagementDb(builder.Configuration);
            builder.Services.AddRepositories();
            builder.Services.AddRepositoryServices();

            WebApplication app = builder.Build();

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
