using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Domain;

// ReSharper disable UnusedAutoPropertyAccessor.Local - JSON serialization

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

[ExcludeFromCodeCoverage]
public sealed partial class CaseReferenceClient : ICaseReferenceClient
{
    private readonly HttpClient _client;

    public CaseReferenceClient(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<CaseDetailModel?> LookupCaseDetails(string caseReference)
    {
        string filter = $"$select=inss_name&$filter=contains(inss_name,'{caseReference}')";
        string url = new($"api/data/v9.0/inss_outboundcreatecasemessages?{filter}"); 

        using HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string contents = await response.Content.ReadAsStringAsync();
            DynamicsMessage? dynamicsMessage = JsonSerializer.Deserialize<DynamicsMessage>(contents);
            CaseMessage? caseMessage = dynamicsMessage?.Value.FirstOrDefault();

            if (caseMessage is not null)
            {
                return new CaseDetailModel
                {
                    CaseReference = caseReference,
                    CompanyName = caseMessage.CompanyName
                };
            }
        }

        return null;
    }
    
    private sealed class DynamicsMessage
    {
        [JsonPropertyName("value")]
        public CaseMessage[] Value { get; init; }
    }

    private sealed partial class CaseMessage
    {
        [JsonPropertyName("inss_name")]
        public string Detail { get; init; }

        public string Reference => ExtractCaseRef();

        public string CompanyName => ExtractCompanyName();

        private string ExtractCaseRef()
        {
            if (string.IsNullOrWhiteSpace(Detail))
            {
                return string.Empty;
            }
        
            var matches = CaseReferenceRegex().Matches(Detail);
            return matches.Count > 0 ? matches[0].Value : string.Empty;
        }

        private string ExtractCompanyName()
        {
            var caseRef = Reference;
        
            if (string.IsNullOrWhiteSpace(Detail) || string.IsNullOrWhiteSpace(caseRef))
            {
                return string.Empty;
            }

            var companyName = Detail.Replace(caseRef, string.Empty).Trim();

            if (companyName.EndsWith("-", StringComparison.InvariantCultureIgnoreCase))
            {
                companyName = companyName.TrimEnd('-');
            }

            return companyName.Trim();
        }

        [GeneratedRegex("CN[0-9]{8}|cn[0-9]{8}|Cn[0-9]{8}|cN[0-9]{8}")]
        private static partial Regex CaseReferenceRegex();
    }
}