using GovUk.Forms.Application.Clients;
using GovUk.Forms.Domain.Search;

namespace GovUk.Forms.Infrastructure.Clients;

public sealed class MockSearchClient : ISearchClient
{
    private static readonly SearchResult[] _searchResultList = CreateMockSearchResults();
    
    public Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        SearchResult[] searchedResults = _searchResultList
            .Where(r => r.Fields["FirstName"].Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) || 
                        r.Fields["FamilyName"].Contains(request.SearchText, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        
        return Task.FromResult(new SearchResponse
        {
            TotalResults = searchedResults.Length,
            Results = searchedResults.Skip(request.Skip).Take(request.PageSize).ToArray()
        });
    }

    private static SearchResult[] CreateMockSearchResults()
    {
        List<SearchResult> searchResults = [];
        Dictionary<string, string> fields = [];
        fields.Add("CaseNumber", "CN10000010");
        fields.Add("Title", "Mr");
        fields.Add("FirstName", "Jim");
        fields.Add("FamilyName", "Smith");
        fields.Add("DateOfBirth", "03-12-2000");
        SearchResult searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("CaseNumber", "CN10000011");
        fields.Add("Title", "Mrs");
        fields.Add("FirstName", "Jenny");
        fields.Add("FamilyName", "Smith");
        fields.Add("DateOfBirth", "20-11-2001");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("CaseNumber", "CN10000012");
        fields.Add("Title", "Mr");
        fields.Add("FirstName", "John");
        fields.Add("FamilyName", "Jones");
        fields.Add("DateOfBirth", "14-10-2002");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("CaseNumber", "CN10000013");
        fields.Add("Title", "Mr");
        fields.Add("FirstName", "Janet");
        fields.Add("FamilyName", "Jones");
        fields.Add("DateOfBirth", "04-08-1999");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("CaseNumber", "CN10000014");
        fields.Add("Title", "Mr");
        fields.Add("FirstName", "Jeffery");
        fields.Add("FamilyName", "Jempson");
        fields.Add("DateOfBirth", "07-04-1998");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("CaseNumber", "CN10000015");
        fields.Add("Title", "Mrs");
        fields.Add("FirstName", "Jane");
        fields.Add("FamilyName", "Jempson");
        fields.Add("DateOfBirth", "13-08-2006");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("CaseNumber", "CN10000014");
        fields.Add("Title", "Mr");
        fields.Add("FirstName", "Jimbo");
        fields.Add("FamilyName", "Jeffers");
        fields.Add("DateOfBirth", "17-05-1994");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        return searchResults.ToArray();
    }
}