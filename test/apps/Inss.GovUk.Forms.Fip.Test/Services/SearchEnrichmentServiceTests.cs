using GovUk.Forms.Application.Clients;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Search;
using Inss.GovUk.Forms.Fip.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.Fip.Test.Services;

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
            Result = new SearchResult { Fields = new Dictionary<string, string> { ["LicensingBody"] = "ACCA" } }
        };
        SearchDetailRequest request = new() { KeyField = "IpNo", KeyValue = "12345678" };
        _searchClient.SearchDetailAsync(request).Returns(expectedResponse);
        
        SearchDetailResponse? actualResponse = await _searchEnrichmentService.SearchDetailAsync(request);
        
        Assert.NotNull(actualResponse);
        Assert.Equal("Association of Chartered Certified Accountants", actualResponse.Result.Fields["AuthBodyName"]);
        Assert.Equal("The Adelphi", actualResponse.Result.Fields["AuthBodyAddressLine1"]);
        Assert.Equal("1-11 John Adam Street", actualResponse.Result.Fields["AuthBodyAddressLine2"]);
        Assert.Equal("Adelphi Terrace", actualResponse.Result.Fields["AuthBodyAddressLine3"]);
        Assert.Equal("London", actualResponse.Result.Fields["AuthBodyAddressLine4"]);
        Assert.Equal(string.Empty, actualResponse.Result.Fields["AuthBodyAddressLine5"]);
        Assert.Equal("WC2N 6AU", actualResponse.Result.Fields["AuthBodyPostcode"]);
        Assert.Equal("1234567890", actualResponse.Result.Fields["Phone"]);
        Assert.Equal("www.somewebsite.com", actualResponse.Result.Fields["Website"]);
    }
}