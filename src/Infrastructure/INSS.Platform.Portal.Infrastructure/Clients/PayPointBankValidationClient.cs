using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Application.Models;
using INSS.Platform.Portal.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
public sealed class PayPointBankValidationClient : IBankClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PayPointBankValidationClient> _logger;
    private readonly BankValidationOptions _options;
    private const string RequestErrorMessage = "An error occurred while verifying bank details. Try again later.";  
    
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="PayPointBankValidationClient"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory used to create <see cref="HttpClient"/> instances.</param>
    /// <param name="logger">The logger instance for logging errors and information.</param>
    /// <param name="options">The options containing PayPoint API configuration.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the required PayPoint API URL or API key configuration is missing.
    /// </exception>
    public PayPointBankValidationClient(IHttpClientFactory httpClientFactory, ILogger<PayPointBankValidationClient> logger, IOptions<BankValidationOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task<BankAccountVerificationResponse> VerifyBankDetailsAsync(BankAccountVerificationRequest request)
    {
        string json = string.Empty;
        try
        {
            json = JsonSerializer.Serialize(request, request.GetType(), _serializerOptions);

            StringContent content = new(json, System.Text.Encoding.UTF8, "application/json");

            HttpRequestMessage requestMessage = new (HttpMethod.Post, _options.BaseApiUrl)
            {
                Content = content
            };
            
            requestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _options.ApiKey);

            HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);
            if (!responseMessage.IsSuccessStatusCode)
            {
                string responseContent = await responseMessage.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error posting user data to {_options.BaseApiUrl}. Response: {responseContent}", null, responseMessage.StatusCode);
            }

            return await MapVerificationResponse(responseMessage);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed calling: {ApiUrl}", _options.BaseApiUrl);
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Request timed out or was canceled calling: {ApiUrl}", _options.BaseApiUrl);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Deserialization failed for JSON: {Json} while posting to {ApiUrl}", json, _options.BaseApiUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while posting form data to {ApiUrl}", _options.BaseApiUrl);
        }

        return new BankAccountVerificationResponse() { ResultText = RequestErrorMessage };
    }

    private async Task<BankAccountVerificationResponse> MapVerificationResponse(HttpResponseMessage responseMessage)
    {
        string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

        BankAccountVerificationResponse response = JsonSerializer.Deserialize<BankAccountVerificationResponse>(jsonResponse, _serializerOptions) 
            ?? throw new JsonException("Deserialization returned null.");
        
        if(! response.Result)
        {
            response.ResultText = response.ReasonCode switch
            {
                "ANNM" or "MBAM" or "BAMM" or "PAMM" => "The name does not match the account.",
                "BANM" => "The name matches, but the account is a business account.",
                "PANM" => "The name matches, but the account is a personal account.",
                "AC01" => "The account number and sort code are not correct. Update and try again.",
                "ACNS" => "We can’t check this account type.",
                "OPTO" => "We can’t check this account because the account holder has opted out of the bank account validation service.",
                "SCNS" => "Sort code not found.  Unable to check the name.",
                "SECMISS" => "Secondary reference missing.",
                "FOURHUN" => "Unable to check the name. Try again later.",
                "HTTPERR" => "HTTP error occured. Unable to check the name. Try again later.",
                "CASS" => "The account has been switched to another bank account provider.",
                "TOOMANY" => "We’re receiving too many requests. Try again later.",
                "NOROUTE" => "We can’t process this request right now. Try again later.",
                _ => response.ResultText,
            };
        }
        
        return response;
    }
}