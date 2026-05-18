using System.Net.Http.Headers;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

public sealed class SubmitIPUploadSectionClient : ISubmitIPUploadSectionClient
{
    private readonly HttpClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SubmitIPUploadSectionClient(HttpClient client, IHttpContextAccessor httpContextAccessor)
    {
        _client = client;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task SubmitAsync(SectionModel section, string userSessionId)
    {
        string? accessToken = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");

        HttpRequestMessage request = new();
        request.RequestUri = new Uri("/ipupload/submit", UriKind.Relative);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        Console.WriteLine("Calling submission service...");
        HttpResponseMessage response = await _client.SendAsync(request);
        
        response.EnsureSuccessStatusCode();
    }
}