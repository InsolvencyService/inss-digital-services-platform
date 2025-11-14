using GovUk.Frontend.AspNetCore;
using INSS.Platform.Portal.Application.Extensions;
using INSS.Platform.Portal.Infrastructure.Extensions;
using INSS.Platform.Portal.Web.Components.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace INSS.Platform.Portal.Web.Components.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddComponents(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllersWithViews()
            .AddApplicationPart(typeof(BaseController<>).Assembly);
        
        builder.Services.AddHttpClient();
        builder.Services.AddGovUkFrontend(options => options.Rebrand = true);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure();
        return builder;
    }
}