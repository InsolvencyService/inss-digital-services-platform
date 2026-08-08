using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.RpsProvider.Extensions;

/*public static class ApplicationBuilderExtensions
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseLoginPage(PagePath path)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                        name: path.Value,
                        pattern: path.Value,
                        defaults: new { controller = "Login", action = "Index" })
                    .WithStaticAssets();   
            });
            return app;
        }
    }
}*/