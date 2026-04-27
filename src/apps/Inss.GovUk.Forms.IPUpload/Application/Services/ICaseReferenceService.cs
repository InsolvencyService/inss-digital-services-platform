namespace Inss.GovUk.Forms.IPUpload.Application.Services;

public interface ICaseReferenceService
{
    Task<bool> CheckExistsAsync(string caseReference);
}