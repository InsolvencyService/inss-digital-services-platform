namespace Inss.Platform.Submission.Service.IPUpload.Clients;

public interface IUploadContentBlobClient
{
    Task<string> GetAsync(string sessionId);
}