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

    private static async Task<HttpRequestMessage> CreateRequestAsync(SubmitIPUploadRequest submitRequest)
    {
        await Task.Delay(10);
        string? accessToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJ0ZXN0QHVzZXIub3JnIiwibmFtZSI6InRlc3RAdXNlci5vcmciLCJzdWIiOiJ0ZXN0QHVzZXIub3JnIiwidW5pcXVlX25hbWUiOiJ0ZXN0QHVzZXIub3JnIiwibm9uY2UiOiI2MzkxNDcwMjgwNjcxMTEwNjkuTURFMk9EWTBOR1V0TUdGa1pDMDBOREV6TFdKa01qZ3RZelpoTURBMFlqUTRaakprWmpCalpUVmtPRGN0T1RjNE1TMDBPVEV4TFRreU5XSXRaRFZrT0dJNU1tSXlOMk0xIiwic3VibWlzc2lvbiI6InN1Ym1pdCIsIm5iZiI6MTc3OTEwNjAwOCwiZXhwIjoxNzc5MzIyMDA4LCJpYXQiOjE3NzkxMDYwMDgsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcwNzMiLCJhdWQiOiIxMjM0NTYifQ.sbXtgUaycyaw6r-cIg_jPe9MXH7EdpHb3_LwPhsvZtVxjGTNpt40nF3vKB5gxwuBL7JkJuvIDg_P_DneGa_TME3spcN_JWNRhQI2ey57i0V6kJr0Jr_I7yUB-Sj794M-Vj-HzwiN7bWB6wYNHvetbLS3L5XhMD9iBszwFMhyPfloKc5In-E_xldE-prSeS6dxo0HHbZDfbPtPqKcFmqNGhwY9z4zbI_gODNfERuOaY5K_bVLhn5swfVIOBso-be76pk0dnCZy8-BnuEuwgdkD2p5fhwt4EZVMwS9Te6mjLBHONwVYoDgyYgzBYeOHeImr7nPRYJn69uO5vb4v0NBaw";//await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
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