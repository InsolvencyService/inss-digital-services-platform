using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Text.Json;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Options;
using Microsoft.Azure.Relay;
using Microsoft.Extensions.Options;

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

[ExcludeFromCodeCoverage]
public sealed class CaseReferenceClient : ICaseReferenceClient
{
    private readonly HttpClient _client;
    private readonly IOptions<RpsApiOptions> _options;

    public CaseReferenceClient(HttpClient client, IOptions<RpsApiOptions> options)
    {
        _client = client;
        _options = options;
    }
    
    public async Task<bool> CheckExistsAsync(string caseReference)
    {
        string token = await GetToken();
        string json = JsonSerializer.Serialize(new { caseRefNumber = caseReference });
        
        using HttpRequestMessage request = new(HttpMethod.Post, _options.Value.ConnectionName);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Headers.Add("ServiceBusAuthorization", token);
        
        HttpResponseMessage response = await _client.SendAsync(request);
        return response.StatusCode == HttpStatusCode.OK;
    }

    private async Task<string> GetToken()
    {
        TokenProvider tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(_options.Value.KeyName, _options.Value.Key);
        var token = await tokenProvider.GetTokenAsync(_client.BaseAddress!.ToString(), TimeSpan.FromMinutes(1));
        return token.TokenString;
    }
}