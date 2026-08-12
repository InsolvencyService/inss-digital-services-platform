using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GovUk.Forms.Components.Extensions;

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