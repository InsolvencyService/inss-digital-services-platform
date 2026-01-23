using INSS.Platform.Canonical.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace INSS.Platform.Portal.Application.Services;


/// <inheritdoc />
public class FormApiClient : IFormApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FormApiClient> _logger;
    private readonly string _apiUrl;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="FormApiClient"/> class with the specified HTTP client and
    /// logger.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> instance used to send HTTP requests to the API.</param>
    /// <param name="configuration">The application configuration settings.</param>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance used for logging diagnostic messages. This parameter
    /// cannot be <see langword="null"/>.</param>
    public FormApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<FormApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;

        _apiUrl = configuration["CanonicalApiUrl"] ?? throw new ArgumentNullException(nameof(configuration), "CanonicalApiUrl configuration is missing.");
    }

    /// <inheritdoc />
    public async Task<bool> PostFormUserDataAsync(User userData)
    {
        if (userData == null)
        {
            return false;
        }

        string userDataApiUrl = $"{_apiUrl}/user";
        string json = string.Empty;
        try
        {
            json = JsonSerializer.Serialize(userData, userData.GetType(), _serializerOptions);

            StringContent content = new(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(userDataApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error posting user data to {_apiUrl}. Response: {responseContent}", null, response.StatusCode);
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
