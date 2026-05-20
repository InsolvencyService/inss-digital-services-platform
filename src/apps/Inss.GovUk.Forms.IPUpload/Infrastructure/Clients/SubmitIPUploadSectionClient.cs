using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Inss.Common;
using Inss.Common.IPUpload;
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
    
    public async Task<Result<SubmitIPUploadResponse>> SubmitAsync(SubmitIPUploadRequest submitRequest)
    {
        using HttpRequestMessage apiRequest = await CreateRequestAsync(submitRequest);
        using HttpResponseMessage apiResponse = await _client.SendAsync(apiRequest);
        return await HandleResponseAsync(apiResponse);
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(SubmitIPUploadRequest submitRequest)
    {
        string? accessToken = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
        HttpRequestMessage apiRequest = new(HttpMethod.Post, "/ipupload/submit");
        apiRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        apiRequest.Content = new StringContent(JsonSerializer.Serialize(submitRequest), Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);
        return apiRequest;
    }

    private static async Task<Result<SubmitIPUploadResponse>> HandleResponseAsync(HttpResponseMessage apiResponse)
    {
        if (!apiResponse.IsSuccessStatusCode)
        {
            return new Error($"Unable to successfully submit the IP upload. Status code {apiResponse.StatusCode}", ErrorType.Unexpected);
        }

        SubmitIPUploadResponse? submitResponse = await apiResponse.Content.ReadFromJsonAsync<SubmitIPUploadResponse>();
        return submitResponse is not null 
            ? (Result<SubmitIPUploadResponse>)submitResponse 
            : new Error("Unable to serialize IP Upload response.", ErrorType.Unexpected);
    }
}