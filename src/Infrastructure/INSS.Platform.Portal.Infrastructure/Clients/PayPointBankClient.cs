using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace INSS.Platform.Portal.Infrastructure.Clients;

/// <summary>
/// Provides functionality to verify bank account details using the PayPoint API.
/// </summary>
/// <remarks>
/// This client requires configuration of the PayPoint API URL and API key, which must be supplied via
/// application configuration. It handles HTTP communication with the PayPoint service and logs errors encountered
/// during requests. Instances of this class are intended to be used for verifying bank account information in scenarios
/// where integration with the PayPoint service is required.
/// </remarks>
public sealed class PayPointBankClient : IBankClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PayPointBankClient> _logger;
    private readonly string _apiUrl;
    private readonly string _apiKey;
    private const string RequestErrorMessage = "An error occurred while verifying bank details. Try again later.";  
    
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="PayPointBankClient"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory used to create <see cref="HttpClient"/> instances.</param>
    /// <param name="configuration">The application configuration containing PayPoint API settings.</param>
    /// <param name="logger">The logger instance for logging errors and information.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the required PayPoint API URL or API key configuration is missing.
    /// </exception>
    public PayPointBankClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<PayPointBankClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;

        _apiUrl = configuration["PayPoint:Api-Url"] ?? throw new ArgumentNullException(nameof(configuration), "PayPoint:Api-Url configuration is missing.");
        _apiKey = configuration["PayPoint:Api-Key"] ?? throw new ArgumentNullException(nameof(configuration), "PayPoint:Api-Key configuration is missing.");
    }

    /// <inheritdoc />
    public async Task<BankAccountVerificationResponse> VerifyBankDetailsAsync(BankAccountVerificationRequest request)
    {
        string json = string.Empty;
        try
        {
            json = JsonSerializer.Serialize(request, request.GetType(), _serializerOptions);

            StringContent content = new(json, System.Text.Encoding.UTF8, "application/json");

            HttpRequestMessage requestMessage = new (HttpMethod.Post, _apiUrl)
            {
                Content = content
            };
            
            requestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);

            HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);
            if (!responseMessage.IsSuccessStatusCode)
            {
                string responseContent = await responseMessage.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error posting user data to {_apiUrl}. Response: {responseContent}", null, responseMessage.StatusCode);
            }

            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();
            BankAccountVerificationResponse? verificationResponse = JsonSerializer.Deserialize<BankAccountVerificationResponse>(jsonResponse, _serializerOptions);

            return verificationResponse ?? new BankAccountVerificationResponse();
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

        return new BankAccountVerificationResponse() { ResultText = RequestErrorMessage };
    }
}