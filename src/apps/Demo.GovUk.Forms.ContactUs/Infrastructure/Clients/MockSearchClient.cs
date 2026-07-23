using GovUk.Forms.Application.Clients;
using GovUk.Forms.Domain.Search;

namespace Demo.GovUk.Forms.ContactUs.Infrastructure.Clients;

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

    public Task<SearchDetailResponse?> SearchDetailAsync(SearchDetailRequest request)
    {
        SearchResult? result = _searchResultList.FirstOrDefault(r => r.Fields[request.KeyField] == request.KeyValue);

        return Task.FromResult(
            result is not null 
                ? new SearchDetailResponse { Result = new SearchResult { Fields = result.Fields } }
                : null);
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
        fields.Add("Phone", "01234112233");
        fields.Add("Email", "jim@smith.com");
        fields.Add("Line1", "101 Ivy Terrace");
        fields.Add("Postcode", "TN33 0DN");
        SearchResult searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("CaseNumber", "CN10000011");
        fields.Add("Title", "Mrs");
        fields.Add("FirstName", "Jenny");
        fields.Add("FamilyName", "Smith");
        fields.Add("DateOfBirth", "20-11-2001");
        fields.Add("Phone", "01234222233");
        fields.Add("Email", "jenny@smith.com");
        fields.Add("Line1", "102 Ivy Terrace");
        fields.Add("Postcode", "TN33 0DN");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("CaseNumber", "CN10000012");
        fields.Add("Title", "Mr");
        fields.Add("FirstName", "John");
        fields.Add("FamilyName", "Jones");
        fields.Add("DateOfBirth", "14-10-2002");
        fields.Add("Phone", "01234332233");
        fields.Add("Email", "john@jones.com");
        fields.Add("Line1", "103 Ivy Terrace");
        fields.Add("Postcode", "TN33 0DN");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("CaseNumber", "CN10000013");
        fields.Add("Title", "Mr");
        fields.Add("FirstName", "Janet");
        fields.Add("FamilyName", "Jones");
        fields.Add("DateOfBirth", "04-08-1999");
        fields.Add("Phone", "01234442233");
        fields.Add("Email", "janet@jones.com");
        fields.Add("Line1", "104 Ivy Terrace");
        fields.Add("Postcode", "TN33 0DN");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("CaseNumber", "CN10000014");
        fields.Add("Title", "Mr");
        fields.Add("FirstName", "Jeffery");
        fields.Add("FamilyName", "Jempson");
        fields.Add("DateOfBirth", "07-04-1998");
        fields.Add("Phone", "01234552233");
        fields.Add("Email", "jeffery@jempson.com");
        fields.Add("Line1", "105 Ivy Terrace");
        fields.Add("Postcode", "TN33 0DN");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("CaseNumber", "CN10000015");
        fields.Add("Title", "Mrs");
        fields.Add("FirstName", "Jane");
        fields.Add("FamilyName", "Jempson");
        fields.Add("DateOfBirth", "13-08-2006");
        fields.Add("Phone", "01234662233");
        fields.Add("Email", "jane@jempson.com");
        fields.Add("Line1", "106 Ivy Terrace");
        fields.Add("Postcode", "TN33 0DN");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("CaseNumber", "CN10000014");
        fields.Add("Title", "Mr");
        fields.Add("FirstName", "Jimbo");
        fields.Add("FamilyName", "Jeffers");
        fields.Add("DateOfBirth", "17-05-1994");
        fields.Add("Phone", "01234772233");
        fields.Add("Email", "jimbo@jeffers.com");
        fields.Add("Line1", "102 Ivy Terrace");
        fields.Add("Postcode", "TN33 0DN");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        return searchResults.ToArray();
    }
}