using Inss.Platform.Submission.Service.Endpoints;

namespace Inss.Platform.Submission.Service.Extensions;

public static class ApplicationBuilderExtensions
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseApi()
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(configure =>
            {
                configure.DefineRootEndpoint();
                configure.DefineHealthEndpoint();
                configure.DefineSubmitIPUploadEndpoint();
            });
            
            return app;
        }
    }
}