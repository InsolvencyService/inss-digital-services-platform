using Inss.Platform.Application.Clients;
using Inss.Platform.Application.Services;
using Inss.Platform.Domain.Components.Searching.Support;
using Inss.Platform.Fip.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Inss.Platform.Fip.Test.Application.Services;

public class SearchEnrichmentServiceTests
{
    private readonly SearchEnrichmentService _searchEnrichmentService;
    private readonly ISearchClient _searchClient;
    private const string ConfigKey = "FIPSearch";

    public SearchEnrichmentServiceTests()
    {
        ServiceCollection services = [];
        services.AddSingleton(Substitute.For<ILogger<SearchService>>());
        _searchClient = Substitute.For<ISearchClient>();
        services.AddKeyedSingleton(ConfigKey, _searchClient);
        _searchEnrichmentService = new SearchEnrichmentService(ConfigKey, services.BuildServiceProvider());
    }
    
    [Fact]
    public async Task SearchingResults_SearchAsync_ReturnsResponseWithNoEnrichment()
    {
        SearchResponse expectedResponse = new();
        SearchRequest request = new() { SearchText = "London", CurrentPageNumber = 1, PageSize = 10 };
        _searchClient.SearchAsync(request).Returns(expectedResponse);
        
        SearchResponse actualResponse = await _searchEnrichmentService.SearchAsync(request);
        
        Assert.Equal(expectedResponse, actualResponse);
    }
    
    [Fact]
    public async Task SearchingResults_SearchDetailAsync_ReturnsResponseWithEnrichment()
    {
        SearchDetailResponse expectedResponse = new()
        {
            Result = new SearchResult { Fields = new Dictionary<string, string> { ["LicensingBody"] = "ICAEW" } }
        };
        SearchDetailRequest request = new() { KeyField = "IpNo", KeyValue = "12345678" };
        _searchClient.SearchDetailAsync(request).Returns(expectedResponse);
        
        SearchDetailResponse? actualResponse = await _searchEnrichmentService.SearchDetailAsync(request);
        
        Assert.NotNull(actualResponse);
        Assert.Equal("Institute of Chartered Accountants in England and Wales", actualResponse.Result.Fields["AuthBodyName"]);
        Assert.Equal("Chartered Accountants’ Hall", actualResponse.Result.Fields["AuthBodyAddressLine1"]);
        Assert.Equal("One Moorgate Place", actualResponse.Result.Fields["AuthBodyAddressLine2"]);
        Assert.Equal(string.Empty, actualResponse.Result.Fields["AuthBodyAddressLine3"]);
        Assert.Equal("London", actualResponse.Result.Fields["AuthBodyAddressLine4"]);
        Assert.Equal(string.Empty, actualResponse.Result.Fields["AuthBodyAddressLine5"]);
        Assert.Equal("EC2R 6EA", actualResponse.Result.Fields["AuthBodyPostcode"]);
        Assert.Equal("01908 248 250", actualResponse.Result.Fields["Phone"]);
        Assert.Equal("https://www.icaew.com/", actualResponse.Result.Fields["Website"]);
    }
}