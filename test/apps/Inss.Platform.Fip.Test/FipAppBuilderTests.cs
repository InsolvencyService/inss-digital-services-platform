using Inss.Platform.Application.Factories;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components.Searching;
using Inss.Platform.Domain.Primitives;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Inss.Platform.Fip.Test;

public class FipAppBuilderTests
{
    [Fact]
    public void RegisteredApp_Build_ReturnsPagePaths()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        FipAppBuilder fipAppBuilder = new();

        PagePath[] pagePaths = fipAppBuilder.Build(builder.Services);
        
        Assert.Equal(3, pagePaths.Length);
        Assert.Equal("/search", pagePaths[0]);
        Assert.Equal("/search-results", pagePaths[1]);
        Assert.Equal("/search-result-detail", pagePaths[2]);
    }
    
    [Fact]
    public void RegisteredApp_Build_RegistersAppFactory()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        FipAppBuilder fipAppBuilder = new();

        fipAppBuilder.Build(builder.Services);

        WebApplication webApp = builder.Build();
        IAppFactory? appFactory = webApp.Services.GetService<IAppFactory>();
        Assert.NotNull(appFactory);
    }
    
    [Fact]
    public async Task RegisteredApp_Build_RegisteredAppFactoryReturnsPages()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        FipAppBuilder fipAppBuilder = new();

        fipAppBuilder.Build(builder.Services);

        WebApplication webApp = builder.Build();
        IAppFactory appFactory = webApp.Services.GetRequiredService<IAppFactory>();
        AppModel app = await appFactory.CreateAsync("Test");
        Assert.NotNull(app.Pages.Find(p => p.Path == "/search"));
        Assert.NotNull(app.Pages.Find(p => p.Path == "/search-results"));
        Assert.NotNull(app.Pages.Find(p => p.Path == "/search-result-detail"));
    }
    
    [Fact]
    public async Task RegisteredApp_Build_RegisteredAppSearchTermPageHasComponent()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        FipAppBuilder fipAppBuilder = new();

        fipAppBuilder.Build(builder.Services);

        WebApplication webApp = builder.Build();
        IAppFactory appFactory = webApp.Services.GetRequiredService<IAppFactory>();
        AppModel app = await appFactory.CreateAsync("Test");
        PageModel page = app.Pages.GetPage("/search");
        Assert.True(page.Components.HasComponent<SearchTermComponentModel>());
    }
    
    [Fact]
    public async Task RegisteredApp_Build_RegisteredAppSearchResultPageHasComponent()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        FipAppBuilder fipAppBuilder = new();

        fipAppBuilder.Build(builder.Services);

        WebApplication webApp = builder.Build();
        IAppFactory appFactory = webApp.Services.GetRequiredService<IAppFactory>();
        AppModel app = await appFactory.CreateAsync("Test");
        PageModel page = app.Pages.GetPage("/search-results");
        Assert.True(page.Components.HasComponent<SearchResultComponentModel>());
    }
    
    [Fact]
    public async Task RegisteredApp_Build_RegisteredAppSearchResultDetailPageHasComponent()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        FipAppBuilder fipAppBuilder = new();

        fipAppBuilder.Build(builder.Services);

        WebApplication webApp = builder.Build();
        IAppFactory appFactory = webApp.Services.GetRequiredService<IAppFactory>();
        AppModel app = await appFactory.CreateAsync("Test");
        PageModel page = app.Pages.GetPage("/search-result-detail");
        Assert.True(page.Components.HasComponent<SearchResultDetailComponentModel>());
    }
}