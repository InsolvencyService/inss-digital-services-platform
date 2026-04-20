namespace Inss.GovUk.Forms.IPUpload.Application.Clients;

public interface ICaseReferenceClient
{
    Task<bool> CheckExistsAsync(string caseReference);
}