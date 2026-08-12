namespace Inss.FormsSubmission.Service.Extensions;

public static class WebHostBuilderContextExtensions
{
    extension(WebHostBuilderContext context)
    {
        public bool UseMock(string key)
        {
            return context.HostingEnvironment.IsDevelopment() &&
                   bool.TryParse(context.Configuration[$"{key}:UseMock"], out bool useMock) && useMock;
        }
    }
}