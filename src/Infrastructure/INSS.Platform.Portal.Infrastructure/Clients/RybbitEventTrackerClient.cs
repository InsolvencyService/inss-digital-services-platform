using INSS.Platform.Portal.Application.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace INSS.Platform.Portal.Infrastructure.Clients;

/// <inheritdoc/>
public class RybbitEventTrackerClient : IEventTrackerClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FormApiClient> _logger;
    private readonly string _apiUrl;
    private readonly string _siteId;
    private readonly bool _enabled;


    public RybbitEventTrackerClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<FormApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;

        _apiUrl = configuration["Rybbit:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "Rybbit:BaseUrl configuration is missing.");
        _siteId = configuration["Rybbit:SiteId"] ?? throw new ArgumentNullException(nameof(configuration), "Rybbit:SiteId configuration is missing.");
        _enabled = bool.Parse(configuration["Rybbit:Enabled"] ?? "false");
    }

    /// <inheritdoc/>
    public async Task TrackEventAsync(string eventType, string eventName, Dictionary<string, object> properties)
    {
        if (!_enabled)
        {
            return;
        }

        Dictionary<string, object> eventData = new()
        {
            ["site_id"] = _siteId,
            ["type"] = eventType,
            ["event_name"] = eventName,
            ["properties"] = JsonSerializer.Serialize(properties)
        };

        string json = string.Empty;
        try
        {
            json = JsonSerializer.Serialize(eventData);

            using StringContent content = new(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PostAsync($"{_apiUrl}/api/track", content);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to track event {EventName}. Status Code: {StatusCode}, Error: {Error}", eventName, response.StatusCode, error);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed calling: {ApiUrl}", _apiUrl);
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Request timed out or was canceled calling: {ApiUrl}", _apiUrl);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Deserialization failed for JSON: {Json} while posting to {ApiUrl}", json, _apiUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while posting form data to {ApiUrl}", _apiUrl);
        }
    }
}
