using System.Net;

namespace Inss.FormsSubmission.Service.IPUpload.Clients;

public interface IDynamicsClient
{
    Task<SubmitResponse> SubmitAsync(JsonMessage jsonMessage, CancellationToken cancellationToken);
}

public class MockDynamicsClient : IDynamicsClient
{
    private readonly HttpClient _client;

    public MockDynamicsClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<SubmitResponse> SubmitAsync(JsonMessage jsonMessage, CancellationToken cancellationToken)
    {
        Console.WriteLine("Calling Dynamics...");
        var response = await _client.GetAsync("/", cancellationToken);
        response.EnsureSuccessStatusCode();
        Console.WriteLine("Dynamics call complete.");
        return new SubmitResponse { StatusCode = response.StatusCode };
    }
}

public sealed class SubmitResponse
{
    public HttpStatusCode StatusCode { get; init; }
    
    public string? Error { get; init; }
}