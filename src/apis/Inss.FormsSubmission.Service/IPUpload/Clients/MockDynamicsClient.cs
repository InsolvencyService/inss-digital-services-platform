using System.Net;
// ReSharper disable UnusedParameter.Local

namespace Inss.FormsSubmission.Service.IPUpload.Clients;

public class MockDynamicsClient : IDynamicsClient
{
    public MockDynamicsClient(HttpClient client)
    {
    }

    public async Task<SubmitResponse> SubmitAsync(JsonMessage jsonMessage, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        
        if (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Calling Dynamics...");
            Console.WriteLine("Dynamics call complete.");
            return new SubmitResponse { StatusCode = HttpStatusCode.OK };
        }
        
        return new SubmitResponse { StatusCode = HttpStatusCode.InternalServerError, Error = "Task cancelled."};
    }
}