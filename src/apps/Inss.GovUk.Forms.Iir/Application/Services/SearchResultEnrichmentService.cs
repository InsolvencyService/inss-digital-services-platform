using System.Text.Json;
using GovUk.Forms.Application.Clients;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inss.GovUk.Forms.Iir.Application.Services;

public sealed class SearchEnrichmentService : ISearchService
{
    private readonly SearchService _searchService;

    public SearchEnrichmentService(string configKey, IServiceProvider serviceProvider)
    {
        ISearchClient searchClient = serviceProvider.GetRequiredKeyedService<ISearchClient>(configKey);
        ILogger<SearchService> logger = serviceProvider.GetRequiredService<ILogger<SearchService>>();
        _searchService = new SearchService(searchClient, logger);
    }
    
    public Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        return _searchService.SearchAsync(request);
    }

    public async Task<SearchDetailResponse?> SearchDetailAsync(SearchDetailRequest request)
    {
        SearchDetailResponse? response = await _searchService.SearchDetailAsync(request);

        if (response is not null)
        {
            SearchResult result = response.Result;
            Enrich(result);
        }

        return response;
    }
    
    private static void Enrich(SearchResult result)
    {
        string? licensingBody = result.Fields.FirstOrDefault(f => f.Key == "LicensingBody").Value;

        if (licensingBody is null)
        {
            return;
        }

        List<AuthorisingBodyLookup> list = JsonSerializer.Deserialize<List<AuthorisingBodyLookup>>(Json) ?? [];

        AuthorisingBodyLookup? authorisingBodyLookup = list.FirstOrDefault(abc => abc.AuthBodyCode == licensingBody);

        if (authorisingBodyLookup is null)
        {
            return;
        }
        
        result.Fields[nameof(authorisingBodyLookup.AuthBodyName)] = authorisingBodyLookup.AuthBodyName;
        result.Fields[nameof(authorisingBodyLookup.AuthBodyAddressLine1)] = authorisingBodyLookup.AuthBodyAddressLine1;
        result.Fields[nameof(authorisingBodyLookup.AuthBodyAddressLine2)] = authorisingBodyLookup.AuthBodyAddressLine2;
        result.Fields[nameof(authorisingBodyLookup.AuthBodyAddressLine3)] = authorisingBodyLookup.AuthBodyAddressLine3;
        result.Fields[nameof(authorisingBodyLookup.AuthBodyAddressLine4)] = authorisingBodyLookup.AuthBodyAddressLine4;
        result.Fields[nameof(authorisingBodyLookup.AuthBodyAddressLine5)] = authorisingBodyLookup.AuthBodyAddressLine5;
        result.Fields[nameof(authorisingBodyLookup.AuthBodyPostcode)] = authorisingBodyLookup.AuthBodyPostcode;
        result.Fields[nameof(authorisingBodyLookup.Phone)] = authorisingBodyLookup.Phone;
        result.Fields[nameof(authorisingBodyLookup.Website)] = authorisingBodyLookup.Website;
    }

    private const string Json = """
                                [
                                  {
                                    "AuthBodyCode": "ICAEW",
                                    "AuthBodyName": "Institute of Chartered Accountants in England and Wales",
                                    "AuthBodyAddressLine1": "Chartered Accountants’ Hall",
                                    "AuthBodyAddressLine2": "One Moorgate Place",
                                    "AuthBodyAddressLine3": "",
                                    "AuthBodyAddressLine4": "London",
                                    "AuthBodyAddressLine5": "",
                                    "AuthBodyPostcode": "EC2R 6EA",
                                    "Phone": "01908 248 250",
                                    "Website": "https://www.icaew.com/"
                                  },
                                  {
                                    "AuthBodyCode": "ICAS",
                                    "AuthBodyName": "The Institute of Chartered Accountants of Scotland",
                                    "AuthBodyAddressLine1": "CA House",
                                    "AuthBodyAddressLine2": "21 Haymarket Yards",
                                    "AuthBodyAddressLine3": "",
                                    "AuthBodyAddressLine4": "Edinburgh",
                                    "AuthBodyAddressLine5": "",
                                    "AuthBodyPostcode": "EH12 5BH",
                                    "Phone": "0131 347 0100",
                                    "Website": "https://www.icas.com/"
                                  },
                                  {
                                    "AuthBodyCode": "IPA",
                                    "AuthBodyName": "Insolvency Practitioners Association",
                                    "AuthBodyAddressLine1": "46 New Broad Street",
                                    "AuthBodyAddressLine2": "",
                                    "AuthBodyAddressLine3": "",
                                    "AuthBodyAddressLine4": "London",
                                    "AuthBodyAddressLine5": "",
                                    "AuthBodyPostcode": "EC2M 1JH",
                                    "Phone": "020 8152 4980",
                                    "Website": "https://www.ipa.uk.com/"
                                  }
                                ]
                                """;

    private sealed class AuthorisingBodyLookup
    {
        public string AuthBodyCode { get; init; } = string.Empty;

        public string AuthBodyName { get; init; } = string.Empty;

        public string AuthBodyAddressLine1 { get; init; } = string.Empty;

        public string AuthBodyAddressLine2 { get; init; } = string.Empty;

        public string AuthBodyAddressLine3 { get; init; } = string.Empty;

        public string AuthBodyAddressLine4 { get; init; } = string.Empty;

        public string AuthBodyAddressLine5 { get; init; } = string.Empty;

        public string AuthBodyPostcode { get; init; } = string.Empty;

        public string Phone { get; init; } = string.Empty;

        public string Website { get; init; } = string.Empty;
    }
}