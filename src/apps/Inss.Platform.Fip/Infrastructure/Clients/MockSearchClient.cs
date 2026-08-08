using System.Text.RegularExpressions;
using Inss.Platform.Application.Clients;
using Inss.Platform.Domain.Components.Searching.Support;

namespace Inss.Platform.Fip.Infrastructure.Clients;

public sealed class MockSearchClient : ISearchClient
{
    private static readonly SearchResult[] _searchResultList = CreateMockSearchResults();
    
    public Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        SearchResult[] searchedResults = _searchResultList
            .Where(r => Regex.IsMatch(r.Fields["RegisteredFirmName"], request.SearchText) ||
                        Regex.IsMatch(r.Fields["RegisteredAddressLine1"], request.SearchText) ||
                        Regex.IsMatch(r.Fields["RegisteredAddressLine2"], request.SearchText) ||
                        Regex.IsMatch(r.Fields["RegisteredAddressLine3"], request.SearchText) ||
                        Regex.IsMatch(r.Fields["RegisteredAddressLine4"], request.SearchText) ||
                        Regex.IsMatch(r.Fields["RegisteredAddressLine5"], request.SearchText) ||
                        Regex.IsMatch(r.Fields["RegisteredPostCode"], request.SearchText))
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
        fields.Add("IpNo", "1020");
        fields.Add("Forenames", "Wit");
        fields.Add("Surname", "Russ");
        fields.Add("RegisteredFirmName", "Waelchi-Hessel");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Sample Town");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "TS1 1AA");
        fields.Add("RegisteredPhone", "0105 531 1032");
        fields.Add("IpEmailAddress", "Wit.Russ@myspace.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "ICAEW");
        fields.Add("RegisteredAddressLine1", "1 Test Street");
        fields.Add("RegisteredAddressLine2", "");
        SearchResult searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1021");
        fields.Add("Forenames", "Berry");
        fields.Add("Surname", "Soane");
        fields.Add("RegisteredFirmName", "Bosco-Emmerich");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Sutton");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "SM73 9KK");
        fields.Add("RegisteredPhone", "07855 983 540");
        fields.Add("IpEmailAddress", "");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "177 Queens Road");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1022");
        fields.Add("Forenames", "Lydia");
        fields.Add("Surname", "Brown");
        fields.Add("RegisteredFirmName", "Spinka, Collier and Champlin");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Wichita");
        fields.Add("RegisteredAddressLine5", "Massachusetts");
        fields.Add("RegisteredPostCode", "E20 1HZ");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "ICAS");
        fields.Add("RegisteredAddressLine1", "697 Bashford Parkway");
        fields.Add("RegisteredAddressLine2", "Bultman");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1023");
        fields.Add("Forenames", "");
        fields.Add("Surname", "Flanaghan");
        fields.Add("RegisteredFirmName", "Franecki LLC");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Blackpool");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "B2 2RR");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "Markos.Flanaghan@miibeian.gov.cn");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "Franecki LLC / 123 Huter House");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1024");
        fields.Add("Forenames", "Stafani");
        fields.Add("Surname", "Dennis");
        fields.Add("RegisteredFirmName", "Sipes Inc");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Sample Town");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "TS1 1AA");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "Stafani.Dennis@meetup.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "ICAS");
        fields.Add("RegisteredAddressLine1", "1 Test Street");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1025");
        fields.Add("Forenames", "Bruce");
        fields.Add("Surname", "Wayne");
        fields.Add("RegisteredFirmName", "Sipes Inc");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Birmingham");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "B5 8TZ");
        fields.Add("RegisteredPhone", "02921602908");
        fields.Add("IpEmailAddress", "shaun.walker@mbicoakley.co.uk");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "ICAEW");
        fields.Add("RegisteredAddressLine1", "42 Sipes Street");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1026");
        fields.Add("Forenames", "JON");
        fields.Add("Surname", "SMITH");
        fields.Add("RegisteredFirmName", "Von, Smith and DuBuque");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Scranton");
        fields.Add("RegisteredAddressLine5", "Colorado");
        fields.Add("RegisteredPostCode", "S7 5LR");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "ICAEW");
        fields.Add("RegisteredAddressLine1", "100 Carioca Park");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1027");
        fields.Add("Forenames", "");
        fields.Add("Surname", "Prose");
        fields.Add("RegisteredFirmName", "Okuneva and Sons");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Birmingham");
        fields.Add("RegisteredAddressLine5", "Florida");
        fields.Add("RegisteredPostCode", "U69 3QS");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "Lydia.Chown@insolvency.gov.uk");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "8308 Macpherson Road");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1028");
        fields.Add("Forenames", "David ");
        fields.Add("Surname", "Rankin-upl");
        fields.Add("RegisteredFirmName", "Creditfix-upl");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Birmingham");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "B3 3FR");
        fields.Add("RegisteredPhone", "123456789");
        fields.Add("IpEmailAddress", "IP33@mail.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "42 Credifix Lane");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1036");
        fields.Add("Forenames", "Linda");
        fields.Add("Surname", "Green");
        fields.Add("RegisteredFirmName", "CS-IP Frim");
        fields.Add("RegisteredAddressLine3", "AddressLine3");
        fields.Add("RegisteredAddressLine4", "London");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "BS13AE");
        fields.Add("RegisteredPhone", "01162127257");
        fields.Add("IpEmailAddress", "some0909240907@mail.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("BodyWebsite", "https://www.google.com"); // Added for testing
        fields.Add("RegisteredAddressLine1", "Bristol");
        fields.Add("RegisteredAddressLine2", "AddressLine2");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1077");
        fields.Add("Forenames", "Jonathan");
        fields.Add("Surname", "Ross");
        fields.Add("RegisteredFirmName", "Klocko-Rice");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "London");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "WC1B");
        fields.Add("RegisteredPhone", "01162127259");
        fields.Add("IpEmailAddress", "Jonathan@insolvency.gov.uk");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "1234");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1080");
        fields.Add("Forenames", "Jim");
        fields.Add("Surname", "Mark");
        fields.Add("RegisteredFirmName", "Goodwin and Sons");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Goodwin and Sons");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "GWS 1");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "ICAEW");
        fields.Add("RegisteredAddressLine1", "IP firm location test");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1081");
        fields.Add("Forenames", "Jon-IP");
        fields.Add("Surname", "Smith");
        fields.Add("RegisteredFirmName", "Von, Smith and DuBuque");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Scranton");
        fields.Add("RegisteredAddressLine5", "Colorado");
        fields.Add("RegisteredPostCode", "S7 5LR");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "ICAS");
        fields.Add("RegisteredAddressLine1", "100 Carioca Park");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1084");
        fields.Add("Forenames", "Zoomer");
        fields.Add("Surname", "Wojak");
        fields.Add("RegisteredFirmName", "");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Sutton");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "SM73 9KK");
        fields.Add("RegisteredPhone", "0121798666");
        fields.Add("IpEmailAddress", "Zoomer.Wojak@creditfix.co.uk");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "177 Queens Road");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1085");
        fields.Add("Forenames", "Ryan");
        fields.Add("Surname", "Barrett");
        fields.Add("RegisteredFirmName", "Goodwin and Sons");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Goodwin and Sons");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "GWS 1");
        fields.Add("RegisteredPhone", "0121654321");
        fields.Add("IpEmailAddress", "Ryanb@bt.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "ICAEW");
        fields.Add("RegisteredAddressLine1", "IP firm location test");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1087");
        fields.Add("Forenames", "Denver");
        fields.Add("Surname", "Ware");
        fields.Add("RegisteredFirmName", "Lagos Enterprise");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "");
        fields.Add("RegisteredPhone", "01322 789012");
        fields.Add("IpEmailAddress", "someDenver@mail.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "SRA");
        fields.Add("RegisteredAddressLine1", "");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1090");
        fields.Add("Forenames", "Ryan");
        fields.Add("Surname", "Barratt");
        fields.Add("RegisteredFirmName", "Bosco-Emmerich");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Sutton");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "SM73 9KK");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "ICAEW");
        fields.Add("RegisteredAddressLine1", "177 Queens Road");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1093");
        fields.Add("Forenames", "Amir");
        fields.Add("Surname", "Khan");
        fields.Add("RegisteredFirmName", "Creditfix-upl");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Birmingham");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "B3 3FR");
        fields.Add("RegisteredPhone", "01215549111");
        fields.Add("IpEmailAddress", "Amirkhan@gmail.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "42 Credifix Lane");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "1096");
        fields.Add("Forenames", "Alvin");
        fields.Add("Surname", "Gibbs");
        fields.Add("RegisteredFirmName", "Bosco-Emmerich");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Sutton");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "SM73 9KK");
        fields.Add("RegisteredPhone", "01245 678902");
        fields.Add("IpEmailAddress", "someGibbs@mail.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "177 Queens Road");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "11002");
        fields.Add("Forenames", "");
        fields.Add("Surname", "Scard");
        fields.Add("RegisteredFirmName", "Bashirian-Lueilwitz");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Los Angeles");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "S21 6ZI");
        fields.Add("RegisteredPhone", "0121 283 8138");
        fields.Add("IpEmailAddress", "");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "62 Truax Point");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "11003");
        fields.Add("Forenames", "Frannie");
        fields.Add("Surname", "Cogzell");
        fields.Add("RegisteredFirmName", "Ankunding-Schaefer");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Cincinnati");
        fields.Add("RegisteredAddressLine5", "Florida");
        fields.Add("RegisteredPostCode", "U3 4TK");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "Frannie.Cogzell@chicagotribune.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "84214 Beilfuss Plaza");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "11009");
        fields.Add("Forenames", "Kathe");
        fields.Add("Surname", "Picford");
        fields.Add("RegisteredFirmName", "Kshlerin-Goyette");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "Z10 7EV");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "Kathe.Picford@marketwatch.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "3 Miller Parkway");
        fields.Add("RegisteredAddressLine2", "Mallard");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "11010");
        fields.Add("Forenames", "Hollie");
        fields.Add("Surname", "Buckell");
        fields.Add("RegisteredFirmName", "Goodwin and Sons");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Goodwin and Sons");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "GWS 1");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "Hollie.Buckell@com.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "IP firm location test");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "11014");
        fields.Add("Forenames", "Lorrie");
        fields.Add("Surname", "Thumim");
        fields.Add("RegisteredFirmName", "Klocko-Rice");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Scranton");
        fields.Add("RegisteredAddressLine5", "Colorado");
        fields.Add("RegisteredPostCode", "S7 5LR");
        fields.Add("RegisteredPhone", "0167 958 7201");
        fields.Add("IpEmailAddress", "Lorrie.Thumim@google.com.au");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "100 Carioca Park");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "11015");
        fields.Add("Forenames", "Petey");
        fields.Add("Surname", "Margetson");
        fields.Add("RegisteredFirmName", "Bosco-Emmerich");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Sutton");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "SM73 9KK");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "Petey.Margetson@woothemes.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "177 Queens Road");
        fields.Add("RegisteredAddressLine2", "");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "11016");
        fields.Add("Forenames", "Meghan");
        fields.Add("Surname", "Dubble");
        fields.Add("RegisteredFirmName", "Reynolds, Bradtke and Farrell");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "Denver");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "DT9 7DQ");
        fields.Add("RegisteredPhone", "");
        fields.Add("IpEmailAddress", "Meghan.Dubble@cmu.edu");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "ICAS");
        fields.Add("RegisteredAddressLine1", "37 Withers Close");
        fields.Add("RegisteredAddressLine2", "Withers Enclave");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        fields = [];
        fields.Add("IpNo", "11017");
        fields.Add("Forenames", "");
        fields.Add("Surname", "Ferebee");
        fields.Add("RegisteredFirmName", "Funk, Pacocha and Breitenberg");
        fields.Add("RegisteredAddressLine3", "");
        fields.Add("RegisteredAddressLine4", "");
        fields.Add("RegisteredAddressLine5", "");
        fields.Add("RegisteredPostCode", "I1 5FD");
        fields.Add("RegisteredPhone", "0111 234 7454");
        fields.Add("IpEmailAddress", "Dalston.Ferebee@jiathis.com");
        fields.Add("IncludeOnInternet", "Yes");
        fields.Add("LicensingBody", "IPA");
        fields.Add("RegisteredAddressLine1", "5 Declaration Pass");
        fields.Add("RegisteredAddressLine2", "Carioca");
        searchResult = new() { Fields = fields };
        searchResults.Add(searchResult);

        
        return searchResults.ToArray();
    }
}