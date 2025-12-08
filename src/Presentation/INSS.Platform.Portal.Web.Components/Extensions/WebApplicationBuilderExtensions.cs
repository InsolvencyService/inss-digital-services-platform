using System.Globalization;
using System.Reflection;
using GovUk.Frontend.AspNetCore;
using INSS.Platform.Portal.Application.Extensions;
using INSS.Platform.Portal.Infrastructure.Extensions;
using INSS.Platform.Portal.Web.Components.Binding;
using INSS.Platform.Portal.Web.Components.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace INSS.Platform.Portal.Web.Components.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddComponents(
        this WebApplicationBuilder builder, 
        Assembly? modelAssembly = null)
    {
        // Ensure UK culture for formatting
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
        
        builder.Services
            .AddControllersWithViews(options =>
            {
                options.ModelBinderProviders.Insert(0, new BaseModelBinderProvider());
            })
            .AddApplicationPart(typeof(FormController).Assembly);
        
        builder.Services.AddHttpClient();
        builder.Services.AddGovUkFrontend(options => options.Rebrand = true);
        builder.Services.AddApplication(options =>
        {
            if (modelAssembly is not null)
            {
                options.AddModelAssembly(modelAssembly);
            }
        });
        builder.Services.AddInfrastructure();
        return builder;
    }
}