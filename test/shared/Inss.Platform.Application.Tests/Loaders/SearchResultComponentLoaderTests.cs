using Inss.Platform.Application.Loaders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Inss.Platform.Application.Tests.Loaders;

public class SearchResultComponentLoaderTests
{
    private readonly SearchResultComponentLoader _searchResultComponentLoader;

    public SearchResultComponentLoaderTests()
    {
        ServiceCollection services = [];
        ILogger<SearchResultComponentLoader> logger = Substitute.For<ILogger<SearchResultComponentLoader>>();
        _searchResultComponentLoader = new SearchResultComponentLoader(services.BuildServiceProvider(), logger);
    }

    // [Fact]
    // public async Task Todo()
    // {
    //     LoaderContext context = new()
    //     {
    //         
    //     }
    // }
}