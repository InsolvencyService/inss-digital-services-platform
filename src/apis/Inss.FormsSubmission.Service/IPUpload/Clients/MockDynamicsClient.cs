using System.Net;

namespace Inss.FormsSubmission.Service.IPUpload.Clients;

public class MockDynamicsClient : IDynamicsClient
{
    private readonly HttpClient _client;

    public MockDynamicsClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<SubmitResponse> SubmitAsync(JsonMessage jsonMessage, CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Calling Dynamics...");
            var response = await _client.GetAsync("/", cancellationToken);
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Dynamics call complete.");
            return new SubmitResponse { StatusCode = response.StatusCode };
        }
        
        return new SubmitResponse { StatusCode = HttpStatusCode.InternalServerError, Error = "Task cancelled."};
    }
}

public class DynamicsClient : IDynamicsClient
{
    private readonly HttpClient _client;

    public DynamicsClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<SubmitResponse> SubmitAsync(JsonMessage jsonMessage, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return new SubmitResponse();
    }
}