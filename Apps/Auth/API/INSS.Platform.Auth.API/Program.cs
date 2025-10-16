using INSS.Platform.Auth.API.Services;
using System.Diagnostics.CodeAnalysis;

namespace INSS.Platform.Auth.API
{
    /// <summary>
    /// Entry point for the Auth API service.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "There is no business logic in this class to test.")]
    public static class Program
    {
        /// <summary>
        /// Configures and runs the Auth API web service.
        /// </summary>
        /// <param name="args">Command-line arguments for the application.</param>
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddHttpClient();
            builder.Services.AddApplicationInsightsTelemetry();

            builder.Services.AddScoped<IAuthService, OneLoginAuthService>();
            builder.Services.AddSingleton<IStateCache, StateCache>();

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
