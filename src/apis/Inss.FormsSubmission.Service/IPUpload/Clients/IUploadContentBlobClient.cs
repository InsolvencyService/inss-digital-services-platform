namespace Inss.FormsSubmission.Service.IPUpload.Clients;

public interface IUploadContentBlobClient
{
    Task<string> GetAsync(string sessionId);
}