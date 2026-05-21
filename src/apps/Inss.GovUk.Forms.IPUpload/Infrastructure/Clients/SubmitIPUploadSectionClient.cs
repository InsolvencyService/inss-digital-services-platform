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
        string? accessToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJ0ZXN0QHVzZXIub3JnIiwibmFtZSI6InRlc3RAdXNlci5vcmciLCJzdWIiOiJ0ZXN0QHVzZXIub3JnIiwidW5pcXVlX25hbWUiOiJ0ZXN0QHVzZXIub3JnIiwibm9uY2UiOiI2MzkxNDk3MjM0NDc3MjIwMjkuT1RZNU56QTBZell0T1dFNFpDMDBOekkzTFdKalltTXRNakkzTWpNNFptTXpZVGhtTXpOaE0ySTFZV010Wm1Oa09TMDBZbVJqTFRnd1pETXRaR05qT0dRMU1EbGhNMkZsIiwic3VibWlzc2lvbiI6InN1Ym1pdCIsIm5iZiI6MTc3OTM3NTU0NywiZXhwIjoxNzc5NTkxNTQ3LCJpYXQiOjE3NzkzNzU1NDcsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcwNzMiLCJhdWQiOiIxMjM0NTYifQ.QMGOmkM7CTtE4kiQjfuzuPqM2ixGnAVkNvWXBlZuYMlFdGvOt8oME5yll59Ovl1j5i-49bsC6ViWRn8A6pr_mr3eZ_Fzsl1WXtLenXoMg5H_97FB8XRlGP9-lpw9p0WjUbFWEwjZbqbeEQQ01TTpp7wTv09GVZf5Z9omnsBUkULwMGeXyQTUtkVcZpqmMW5A876TrBkjZdXwftQuYxT91tpuoOsVrZfz90G-qW39I3BfAJsCWaxIs--bWMSOKkxR2ecuV0PXPRzrSK-fdmL9WP8XPNV34J895PhdOeJTkYdY4Wm4U89gl73cP_SLdhwHkvSUjrlGjFVxPL97IRrJvg";//await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
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