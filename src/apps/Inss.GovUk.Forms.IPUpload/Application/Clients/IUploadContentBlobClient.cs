namespace Inss.GovUk.Forms.IPUpload.Application.Clients;

public interface IUploadContentBlobClient
{
    Task<string> GetAsync(string sessionId);
    Task SaveAsync(string xml, string sessionId);
}