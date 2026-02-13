using Azure.Messaging.EventGrid;
using INSS.Platform.Audit.Domain;
using INSS.Platform.Audit.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace INSS.Platform.Audit.Functions;

public class AuditToLogAnalytics
{
    private readonly LogAnalyticsClient _law;
    private readonly ILogger _logger;

    public AuditToLogAnalytics(LogAnalyticsClient law, ILoggerFactory loggerFactory)
    {
        _law = law;
        _logger = loggerFactory.CreateLogger<AuditToLogAnalytics>();
    }

    [Function("AuditToLogAnalytics")]
    public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent)
    {
        try
        {
            AuditEntry? auditEntry = eventGridEvent.Data.ToObjectFromJson<AuditEntry>();

            string jsonPayload = JsonSerializer.Serialize(new[] { auditEntry });

            _logger.LogInformation("Received Event Grid event: {JsonPayload}", jsonPayload);

            // Send to Log Analytics as "AuditEvents"
            await _law.SendLogAsync("AuditEvents", jsonPayload);

            _logger.LogInformation("Sent event to Log Analytics.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process Event Grid event");
            throw;
        }
    }
}