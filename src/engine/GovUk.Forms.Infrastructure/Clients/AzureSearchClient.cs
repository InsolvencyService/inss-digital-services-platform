using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using GovUk.Forms.Application.Clients;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Search;
using GovUk.Forms.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Infrastructure.Clients;

public sealed class AzureSearchClient : ISearchClient
{
    private readonly ILogger<SearchService> _logger;
    private readonly SearchClient _searchClient;

    public AzureSearchClient(SearchClient searchClient, ILogger<SearchService> logger)
    {
        _logger = logger;
        _searchClient = searchClient;
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        SearchOptions searchOptions = new() { Size = request.PageSize, Skip = request.Skip, IncludeTotalCount = true };

        Response<SearchResults<SearchDocument>> response = 
            await _searchClient.SearchAsync<SearchDocument>(request.SearchText, searchOptions);

        int statusCode = response.GetRawResponse().Status;

        if (statusCode is < 200 or > 299)
        {
            _logger.AzureSearchFailed(statusCode, request.SearchText);

            return new SearchResponse();
        }

        List<SearchResult> results = [];

        await foreach (SearchResult<SearchDocument> result in response.Value.GetResultsAsync())
        {
            Dictionary<string, string> fields = result.Document
                .ToDictionary(
                    field => field.Key,
                    field => field.Value?.ToString() ?? string.Empty);

            results.Add(new SearchResult
            {
                Fields = fields
            });
        }

        return new SearchResponse
        {
            Results = [.. results],
            TotalResults = (int)(response.Value.TotalCount ?? 0)
        };
    }
}

public sealed class MockSearchClient : ISearchClient
{
    private static readonly SearchResult[] _results;

    static MockSearchClient()
    {
        List<SearchResult> results = [];
        Dictionary<string, string> x = [];
        x.Add("CaseNumber", "CN10000010");
        x.Add("Title", "Mr");
        x.Add("FirstName", "Jim");
        x.Add("FamilyName", "Smith");
        x.Add("DateOfBirth", "03-12-2000");
        SearchResult sr = new() { Fields = x };
        results.Add(sr);
        
        x = [];
        x.Add("CaseNumber", "CN10000011");
        x.Add("Title", "Mrs");
        x.Add("FirstName", "Jenny");
        x.Add("FamilyName", "Smith");
        x.Add("DateOfBirth", "20-11-2001");
        sr = new SearchResult { Fields = x };
        results.Add(sr);

        x = [];
        x.Add("CaseNumber", "CN10000012");
        x.Add("Title", "Mr");
        x.Add("FirstName", "John");
        x.Add("FamilyName", "Jones");
        x.Add("DateOfBirth", "14-10-2002");
        sr = new SearchResult { Fields = x };
        results.Add(sr);
        
        x = [];
        x.Add("CaseNumber", "CN10000013");
        x.Add("Title", "Mr");
        x.Add("FirstName", "Janet");
        x.Add("FamilyName", "Jones");
        x.Add("DateOfBirth", "04-08-1999");
        sr = new SearchResult { Fields = x };
        results.Add(sr);
        
        x = [];
        x.Add("CaseNumber", "CN10000014");
        x.Add("Title", "Mr");
        x.Add("FirstName", "Jeffery");
        x.Add("FamilyName", "Jempson");
        x.Add("DateOfBirth", "07-04-1998");
        sr = new SearchResult { Fields = x };
        results.Add(sr);

        x = [];
        x.Add("CaseNumber", "CN10000015");
        x.Add("Title", "Mrs");
        x.Add("FirstName", "Jane");
        x.Add("FamilyName", "Jempson");
        x.Add("DateOfBirth", "13-08-2006");
        sr = new SearchResult { Fields = x };
        results.Add(sr);
        
        x = [];
        x.Add("CaseNumber", "CN10000014");
        x.Add("Title", "Mr");
        x.Add("FirstName", "Jimbo");
        x.Add("FamilyName", "Jeffers");
        x.Add("DateOfBirth", "17-05-1994");
        sr = new SearchResult { Fields = x };
        results.Add(sr);
        
        _results = results.ToArray();
    }
    
    public Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        return Task.FromResult(new SearchResponse
        {
            TotalResults = _results.Length,
            Results = _results.Skip(request.Skip).Take(request.PageSize).ToArray()
        });
    }
}
