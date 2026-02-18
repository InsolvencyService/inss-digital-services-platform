using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Messaging.EventGrid;
using INSS.Platform.Audit.Application.Options;
using INSS.Platform.Audit.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace INSS.Platform.Audit.Infrastructure;

/// <summary>
/// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
/// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
/// In a properly defined application the events would be documented and also adhere to a defined contract.
/// </summary>
/// <remarks>
/// Provides an implementation of <see cref="IAuditService"/> that publishes audit events to Azure Event Grid.
/// </remarks>
public sealed class EventGridAuditService : IAuditService
{
    /// <summary>
    /// The Event Grid publisher client used to send events.
    /// </summary>
    private readonly EventGridPublisherClient _client;

    /// <summary>
    /// The audit options containing configuration for Event Grid.
    /// </summary>
    private readonly AuditOptions _options;

    /// <summary>
    /// The logger instance for logging information and errors.
    /// </summary>
    private readonly ILogger<EventGridAuditService> _logger;

    /// <summary>
    /// The retry policy for handling transient failures when publishing events.
    /// </summary>
    private readonly AsyncRetryPolicy _retryPolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventGridAuditService"/> class.
    /// </summary>
    /// <param name="options">The audit options for Event Grid configuration.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="managedIdentity">Optional managed identity credential for authentication.</param>
    public EventGridAuditService(
        IOptions<AuditOptions> options,
        ILogger<EventGridAuditService> logger,
        TokenCredential? managedIdentity = null)
    {
        _options = options.Value;
        _logger = logger;

        _client = !string.IsNullOrEmpty(_options.AccessKey)
            ? new EventGridPublisherClient(
                new Uri(_options.TopicEndpoint),
                new AzureKeyCredential(_options.AccessKey))
            : new EventGridPublisherClient(
                new Uri(_options.TopicEndpoint),
                managedIdentity ?? new DefaultAzureCredential());

        _retryPolicy = Policy
            .Handle<RequestFailedException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)),
                onRetry: (ex, delay, attempt, ctx) =>
                    _logger.LogWarning(ex, "EventGrid publish retry {Attempt} after {Delay}ms", attempt, delay.TotalMilliseconds)
            );
    }

    /// <summary>
    /// Records an audit entry by publishing it as an event to Azure Event Grid.
    /// </summary>
    /// <param name="auditEntry">The audit entry to record.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task RecordAsync(AuditEntry auditEntry, CancellationToken cancellationToken = default)
    {
        try
        {
            EventGridEvent egEvent = new(
                subject: $"{_options.SubjectPrefix}{auditEntry.EventType}",
                eventType: auditEntry.EventType,
                dataVersion: "1.0",
                data: auditEntry 
            );

            await _retryPolicy.ExecuteAsync(
                async _ => await _client.SendEventAsync(egEvent, cancellationToken), new Context());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish audit event {EventType}", auditEntry.EventType);
        }
    }
}
