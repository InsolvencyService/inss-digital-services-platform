using System.Net;
using Inss.FormsSubmission.Service.Extensions;

namespace Inss.FormsSubmission.Service.IPUpload.Clients;

public class DynamicsClient : IDynamicsClient
{
    private readonly HttpClient _client;
    private readonly ILogger<DynamicsClient> _logger;

    public DynamicsClient(HttpClient client, ILogger<DynamicsClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<SubmitResponse> SubmitAsync(JsonMessage jsonMessage, CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            string url = new($"api/data/v9.0/{jsonMessage.Entity}");
            object body = new
            {
                inss_correlationid = jsonMessage.CorrelationId,
                inss_message = jsonMessage.Json,
                inss_name = jsonMessage.MessageName,
                inss_retrycount = 0
            };
            HttpResponseMessage response = await _client.PostAsJsonAsync(url, body, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.SuccessfulSubmissionToDynamics(jsonMessage.CorrelationId);
                return new SubmitResponse { StatusCode = response.StatusCode };
            }

            _logger.FailedSubmissionToDynamics(jsonMessage.CorrelationId);
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return new SubmitResponse { StatusCode = response.StatusCode, Error = responseContent };
        }
        
        return new SubmitResponse { StatusCode = HttpStatusCode.InternalServerError, Error = "Task cancelled."};
    }
}