namespace Inss.Platform.Submission.Service.IPUpload.Clients;

public interface IDynamicsClient
{
    Task<SubmitResponse> SubmitAsync(JsonMessage jsonMessage, CancellationToken cancellationToken);
}