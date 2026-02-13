using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace INSS.Platform.Audit.Functions.Models;

public class LogAnalyticsClient
{
    private readonly HttpClient _http;
    private readonly ILogger _logger;
    private readonly string _workspaceId;
    private readonly string _sharedKey;

    public LogAnalyticsClient(HttpClient http, ILoggerFactory logger)
    {
        _http = http;
        _logger = logger.CreateLogger<LogAnalyticsClient>();

        _workspaceId = Environment.GetEnvironmentVariable("LAW_WORKSPACE_ID")
            ?? throw new InvalidOperationException("LAW_WORKSPACE_ID is not configured");

        _sharedKey = Environment.GetEnvironmentVariable("LAW_SHARED_KEY")
            ?? throw new InvalidOperationException("LAW_SHARED_KEY is not configured");

        _logger.LogInformation("LogAnalyticsClient initialised with Workspace {WorkspaceId}", _workspaceId);
    }

    public async Task SendLogAsync(string logType, string json)
    {
        string dateString = DateTime.UtcNow.ToString("r");

        _logger.LogInformation("Sending json to Log Analytics:\n{Json}", json);

        // Compute signature
        string signature = BuildSignature(json, dateString);

        string url = $"https://{_workspaceId}.ods.opinsights.azure.com/api/logs?api-version=2016-04-01";

        // Properly encode the body
        byte[] bodyBytes = Encoding.UTF8.GetBytes(json);
        ByteArrayContent content = new (bodyBytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        // Build request
        HttpRequestMessage req = new (HttpMethod.Post, url);
        req.Headers.Add("Authorization", signature);
        req.Headers.Add("Log-Type", logType);
        req.Headers.Add("x-ms-date", dateString);
        req.Content = content;

        _logger.LogInformation("Sending {Length} bytes to Log Analytics", bodyBytes.Length);

        HttpResponseMessage response = await _http.SendAsync(req);

        if (!response.IsSuccessStatusCode)
        {
            string respBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to send logs. Status {Status}, Body: {Body}", response.StatusCode, respBody);
        }

        response.EnsureSuccessStatusCode();
    }

    private string BuildSignature(string message, string dateString)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        string stringToSign =
            $"POST\n{messageBytes.Length}\napplication/json\nx-ms-date:{dateString}\n/api/logs";

        byte[] keyBytes = Convert.FromBase64String(_sharedKey);
        ASCIIEncoding encoding = new ();
        byte[] stringToSignBytes = encoding.GetBytes(stringToSign);

        using HMACSHA256 hmac = new (keyBytes);
        string hashedString = Convert.ToBase64String(hmac.ComputeHash(stringToSignBytes));

        return $"SharedKey {_workspaceId}:{hashedString}";
    }
}