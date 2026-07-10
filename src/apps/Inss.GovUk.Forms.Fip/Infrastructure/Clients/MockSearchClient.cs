using GovUk.Forms.Application.Clients;
using GovUk.Forms.Domain.Search;

namespace Inss.GovUk.Forms.Fip.Infrastructure.Clients;

public sealed class MockSearchClient : ISearchClient
{
    private static readonly SearchResult[] _searchResultList = CreateMockSearchResults();
    
    public Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        SearchResult[] searchedResults = _searchResultList
            .Where(r => r.Fields["Location"].Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) || 
                        r.Fields["CompanyName"].Contains(request.SearchText, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        
        return Task.FromResult(new SearchResponse
        {
            TotalResults = searchedResults.Length,
            Results = searchedResults.Skip(request.Skip).Take(request.PageSize).ToArray()
        });
    }

    public Task<SearchDetailResponse?> SearchDetailAsync(SearchDetailRequest request)
    {
        SearchResult? result = _searchResultList.FirstOrDefault(r => r.Fields["Name"] == request.Key);

        return Task.FromResult(
            result is not null 
                ? new SearchDetailResponse { Result = new SearchResult { Fields = result.Fields } }
                : null);
    }

    private static SearchResult[] CreateMockSearchResults()
    {
        List<SearchResult> searchResults = [];
        Dictionary<string, string> fields = [];
        fields.Add("Name", "Alex Carter");
        fields.Add("Company", "Northpoint Insolvency");
        fields.Add("Location", "London");
        fields.Add("Postcode", "EC1A 1BB");
        fields.Add("IPName", "Dave Trotter");
        fields.Add("CompanyName", "Trotter Insolvency Services");
        fields.Add("CompanyAddress", "102 Ivy Terrace");
        fields.Add("Email", "dave@trotter.com");
        fields.Add("Telephone", "01234222222");
        fields.Add("IPNumber", "44332211");
        fields.Add("BodyName", "Insolvency Practitioners Association");
        fields.Add("BodyAddress", "1 New Broad Street");
        fields.Add("BodyWebsite", "https://insolvency-practitioners.org.uk");
        fields.Add("BodyTelephone", "020 8152 4980");
        SearchResult searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("Name", "Beatrice Jones");
        fields.Add("Company", "Thames Recovery");
        fields.Add("Location", "London");
        fields.Add("Postcode", "WC2N 5DU");
        fields.Add("IPName", "Simon Smith");
        fields.Add("CompanyName", "Smith Insolvency Services");
        fields.Add("CompanyAddress", "101 Ivy Terrace");
        fields.Add("Email", "simon@smiths.com");
        fields.Add("Telephone", "01234111111");
        fields.Add("IPNumber", "11223344");
        fields.Add("BodyName", "Insolvency Practitioners Association");
        fields.Add("BodyAddress", "1 New Broad Street");
        fields.Add("BodyWebsite", "https://insolvency-practitioners.org.uk");
        fields.Add("BodyTelephone", "020 8152 4980");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("Name", "Caleb Singh");
        fields.Add("Company", "City Restructure");
        fields.Add("Location", "London");
        fields.Add("Postcode", "SW1A 1AA");
        fields.Add("IPName", "Simon Smith");
        fields.Add("CompanyName", "Smith Insolvency Services");
        fields.Add("CompanyAddress", "101 Ivy Terrace");
        fields.Add("Email", "simon@smiths.com");
        fields.Add("Telephone", "01234111111");
        fields.Add("IPNumber", "11223344");
        fields.Add("BodyName", "Insolvency Practitioners Association");
        fields.Add("BodyAddress", "1 New Broad Street");
        fields.Add("BodyWebsite", "https://insolvency-practitioners.org.uk");
        fields.Add("BodyTelephone", "020 8152 4980");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("Name", "Diana Patel");
        fields.Add("Company", "Capital Administrators");
        fields.Add("Location", "London");
        fields.Add("Postcode", "E1 6AN");
        fields.Add("IPName", "Dave Trotter");
        fields.Add("CompanyName", "Trotter Insolvency Services");
        fields.Add("CompanyAddress", "102 Ivy Terrace");
        fields.Add("Email", "dave@trotter.com");
        fields.Add("Telephone", "01234222222");
        fields.Add("IPNumber", "44332211");
        fields.Add("BodyName", "Insolvency Practitioners Association");
        fields.Add("BodyAddress", "1 New Broad Street");
        fields.Add("BodyWebsite", "https://insolvency-practitioners.org.uk");
        fields.Add("BodyTelephone", "020 8152 4980");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("Name", "Ethan Brooks");
        fields.Add("Company", "Riverbank Advisors");
        fields.Add("Location", "London");
        fields.Add("Postcode", "N1 9GU");
        fields.Add("IPName", "Simon Smith");
        fields.Add("CompanyName", "Smith Insolvency Services");
        fields.Add("CompanyAddress", "101 Ivy Terrace");
        fields.Add("Email", "simon@smiths.com");
        fields.Add("Telephone", "01234111111");
        fields.Add("IPNumber", "11223344");
        fields.Add("BodyName", "Insolvency Practitioners Association");
        fields.Add("BodyAddress", "1 New Broad Street");
        fields.Add("BodyWebsite", "https://insolvency-practitioners.org.uk");
        fields.Add("BodyTelephone", "020 8152 4980");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("Name", "Fiona Clarke");
        fields.Add("Company", "Union Insolvency");
        fields.Add("Location", "London");
        fields.Add("Postcode", "SE1 7PB");
        fields.Add("IPName", "Dave Trotter");
        fields.Add("CompanyName", "Trotter Insolvency Services");
        fields.Add("CompanyAddress", "102 Ivy Terrace");
        fields.Add("Email", "dave@trotter.com");
        fields.Add("Telephone", "01234222222");
        fields.Add("IPNumber", "44332211");
        fields.Add("BodyName", "Insolvency Practitioners Association");
        fields.Add("BodyAddress", "1 New Broad Street");
        fields.Add("BodyWebsite", "https://insolvency-practitioners.org.uk");
        fields.Add("BodyTelephone", "020 8152 4980");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        fields = [];
        fields.Add("Name", "George Ahmed");
        fields.Add("Company", "Lakeside Recovery");
        fields.Add("Location", "London");
        fields.Add("Postcode", "N7 8XS");
        fields.Add("IPName", "Simon Smith");
        fields.Add("CompanyName", "Smith Insolvency Services");
        fields.Add("CompanyAddress", "101 Ivy Terrace");
        fields.Add("Email", "simon@smiths.com");
        fields.Add("Telephone", "01234111111");
        fields.Add("IPNumber", "11223344");
        fields.Add("BodyName", "Insolvency Practitioners Association");
        fields.Add("BodyAddress", "1 New Broad Street");
        fields.Add("BodyWebsite", "https://insolvency-practitioners.org.uk");
        fields.Add("BodyTelephone", "020 8152 4980");
        searchResult = new SearchResult { Fields = fields };
        searchResults.Add(searchResult);
        
        return searchResults.ToArray();
    }
}