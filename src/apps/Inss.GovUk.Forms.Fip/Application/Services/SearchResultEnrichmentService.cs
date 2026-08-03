using System.Text.Json;
using GovUk.Forms.Application.Clients;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inss.GovUk.Forms.Fip.Application.Services;

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
        
        result.Fields.Add(nameof(authorisingBodyLookup.AuthBodyName), authorisingBodyLookup.AuthBodyName);
        result.Fields.Add(nameof(authorisingBodyLookup.AuthBodyAddressLine1), authorisingBodyLookup.AuthBodyAddressLine1);
        result.Fields.Add(nameof(authorisingBodyLookup.AuthBodyAddressLine2), authorisingBodyLookup.AuthBodyAddressLine2);
        result.Fields.Add(nameof(authorisingBodyLookup.AuthBodyAddressLine3), authorisingBodyLookup.AuthBodyAddressLine3);
        result.Fields.Add(nameof(authorisingBodyLookup.AuthBodyAddressLine4), authorisingBodyLookup.AuthBodyAddressLine4);
        result.Fields.Add(nameof(authorisingBodyLookup.AuthBodyAddressLine5), authorisingBodyLookup.AuthBodyAddressLine5);
        result.Fields.Add(nameof(authorisingBodyLookup.AuthBodyPostcode), authorisingBodyLookup.AuthBodyPostcode);
        result.Fields.Add(nameof(authorisingBodyLookup.Phone), authorisingBodyLookup.Phone);
        result.Fields.Add(nameof(authorisingBodyLookup.Website), authorisingBodyLookup.Website);
    }

    private const string Json = """
                                [
                                  {
                                    "AuthBodyCode": "ACCA",
                                    "AuthBodyName": "Association of Chartered Certified Accountants",
                                    "AuthBodyAddressLine1": "The Adelphi",
                                    "AuthBodyAddressLine2": "1-11 John Adam Street",
                                    "AuthBodyAddressLine3": "Adelphi Terrace",
                                    "AuthBodyAddressLine4": "London",
                                    "AuthBodyAddressLine5": "",
                                    "AuthBodyPostcode": "WC2N 6AU",
                                    "Phone": "1234567890",
                                    "Website": "https://www.somewebsite.com"
                                  },
                                  {
                                    "AuthBodyCode": "CAI",
                                    "AuthBodyName": "Chartered Accountants Ireland",
                                    "AuthBodyAddressLine1": "Chartered Accountants House",
                                    "AuthBodyAddressLine2": "47-49 Pearse Street",
                                    "AuthBodyAddressLine3": "",
                                    "AuthBodyAddressLine4": "Dublin 2",
                                    "AuthBodyAddressLine5": "",
                                    "AuthBodyPostcode": "D02 YN40",
                                    "Phone": "1234567890",
                                    "Website": "https://www.somewebsite.com"
                                  },
                                  {
                                    "AuthBodyCode": "ICAEW",
                                    "AuthBodyName": "Institute of Chartered Accountants in England and Wales",
                                    "AuthBodyAddressLine1": "Chartered Accountants’ Hall",
                                    "AuthBodyAddressLine2": "Moorgate Place",
                                    "AuthBodyAddressLine3": "",
                                    "AuthBodyAddressLine4": "London",
                                    "AuthBodyAddressLine5": "",
                                    "AuthBodyPostcode": "EC2R 6EA",
                                    "Phone": "1234567890",
                                    "Website": "https://www.somewebsite.com"
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
                                    "Phone": "1234567890",
                                    "Website": "https://www.somewebsite.com"
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
                                    "Phone": "1234567890",
                                    "Website": "https://www.somewebsite.com"
                                  },
                                  {
                                    "AuthBodyCode": "LSS",
                                    "AuthBodyName": "The Law Society Scotland",
                                    "AuthBodyAddressLine1": "Atria One",
                                    "AuthBodyAddressLine2": "Morrison Street",
                                    "AuthBodyAddressLine3": "",
                                    "AuthBodyAddressLine4": "Edinburgh",
                                    "AuthBodyAddressLine5": "",
                                    "AuthBodyPostcode": "EH3 8EX",
                                    "Phone": "1234567890",
                                    "Website": ""
                                  },
                                  {
                                    "AuthBodyCode": "SRA",
                                    "AuthBodyName": "The Law Society",
                                    "AuthBodyAddressLine1": "113 Chancery Lane",
                                    "AuthBodyAddressLine2": "",
                                    "AuthBodyAddressLine3": "",
                                    "AuthBodyAddressLine4": "London",
                                    "AuthBodyAddressLine5": "",
                                    "AuthBodyPostcode": "WC2A 1PL",
                                    "Phone": "1234567890",
                                    "Website": "https://www.somewebsite.com"
                                  },
                                  {
                                    "AuthBodyCode": "SS",
                                    "AuthBodyName": "Secretary of State",
                                    "AuthBodyAddressLine1": "Department for Business, Energy and Industrial Strategy",
                                    "AuthBodyAddressLine2": "1 Victoria Street",
                                    "AuthBodyAddressLine3": "",
                                    "AuthBodyAddressLine4": "London",
                                    "AuthBodyAddressLine5": "",
                                    "AuthBodyPostcode": "SW1H 0ET",
                                    "Phone": "1234567890",
                                    "Website": "https://www.somewebsite.com"
                                  },
                                  {
                                    "AuthBodyCode": "XCAN",
                                    "AuthBodyName": "Chartered Accountants Regulatory Board",
                                    "AuthBodyAddressLine1": "Chartered Accountants’ Hall",
                                    "AuthBodyAddressLine2": "Moorgate Place",
                                    "AuthBodyAddressLine3": "",
                                    "AuthBodyAddressLine4": "London",
                                    "AuthBodyAddressLine5": "",
                                    "AuthBodyPostcode": "EC2R 6EA",
                                    "Phone": "1234567890",
                                    "Website": "https://www.somewebsite.com"
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