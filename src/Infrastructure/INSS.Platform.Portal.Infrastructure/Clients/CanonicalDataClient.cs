using INSS.Platform.Canonical.Domain;
using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace INSS.Platform.Portal.Infrastructure.Clients;


/// <inheritdoc />
public class CanonicalDataClient : ICanonicalDataClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CanonicalDataClient> _logger;
    private readonly CanonicalDataOptions _options;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="CanonicalDataClient"/> class with the specified HTTP client and
    /// logger.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> instance used to send HTTP requests to the API.</param>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance used for logging diagnostic messages. This parameter cannot be <see langword="null"/>.</param>
    /// <param name="options">The options for configuring the Canonical API client.</param>
    public CanonicalDataClient(IHttpClientFactory httpClientFactory, ILogger<CanonicalDataClient> logger, IOptions<CanonicalDataOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task<bool> PostUserDataAsync(User userData)
    {
        if (userData == null)
        {
            return false;
        }

        string userDataApiUrl = $"{_options.BaseApiUrl}/user";
        string json = string.Empty;
        try
        {
            json = JsonSerializer.Serialize(userData, userData.GetType(), _serializerOptions);

            StringContent content = new(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(userDataApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error posting user data to {_options.BaseApiUrl}. Response: {responseContent}", null, response.StatusCode);
            }

            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed calling: {ApiUrl}", userDataApiUrl);
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Request timed out or was canceled calling: {ApiUrl}", userDataApiUrl);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Deserialization failed for JSON: {Json} while posting to {ApiUrl}", json, userDataApiUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while posting form data to {ApiUrl}", userDataApiUrl);
        }

        return false;
    }
}
