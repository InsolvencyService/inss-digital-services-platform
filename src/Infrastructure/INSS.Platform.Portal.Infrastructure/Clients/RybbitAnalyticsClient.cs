using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace INSS.Platform.Portal.Infrastructure.Clients;

/// <inheritdoc/>
public class RybbitAnalyticsClient : IAnalyticsClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CanonicalDataClient> _logger;
    private readonly AnalyticsOptions _options;

    public RybbitAnalyticsClient(IHttpClientFactory httpClientFactory, ILogger<CanonicalDataClient> logger, IOptions<AnalyticsOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _options = options.Value;
    }

    /// <inheritdoc/>
    public async Task TrackEventAsync(string eventType, string eventName, Dictionary<string, object> properties)
    {
        if (!_options.Enabled)
        {
            return;
        }

        Dictionary<string, object> eventData = new()
        {
            ["site_id"] = _options.SiteId,
            ["type"] = eventType,
            ["event_name"] = eventName,
            ["properties"] = JsonSerializer.Serialize(properties)
        };

        string json = string.Empty;
        try
        {
            json = JsonSerializer.Serialize(eventData);

            using StringContent content = new(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PostAsync($"{_options.BaseUrl}/api/track", content);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to track event {EventName}. Status Code: {StatusCode}, Error: {Error}", eventName, response.StatusCode, error);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed calling: {ApiUrl}", _options.BaseUrl);
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Request timed out or was canceled calling: {ApiUrl}", _options.BaseUrl);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Deserialization failed for JSON: {Json} while posting to {ApiUrl}", json, _options.BaseUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while posting form data to {ApiUrl}", _options.BaseUrl);
        }
    }
}
